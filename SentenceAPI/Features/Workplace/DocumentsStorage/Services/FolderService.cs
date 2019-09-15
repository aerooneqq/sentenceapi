using DataAccessLayer.CommonInterfaces;
using DataAccessLayer.Configuration;
using DataAccessLayer.Configuration.Interfaces;
using DataAccessLayer.DatabasesManager;
using DataAccessLayer.Exceptions;
using DataAccessLayer.Filters;

using SentenceAPI.ApplicationFeatures.Loggers.Interfaces;
using SentenceAPI.ApplicationFeatures.Loggers.Models;
using SentenceAPI.Extensions;
using SentenceAPI.FactoriesManager.Interfaces;
using SentenceAPI.Features.Workplace.DocumentsStorage.Interfaces;
using SentenceAPI.Features.Workplace.DocumentsStorage.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentenceAPI.Features.Workplace.DocumentsStorage.Services
{
    public class FolderService : IFolderService
    {
        #region Static fields
        private static readonly LogConfiguration logConfiguration = new LogConfiguration()
        {
            ServiceName = typeof(FolderService).Name,
            ControllerName = string.Empty
        };
        private static readonly string databaseConfigFile = "mongo_database_config.json";
        #endregion

        #region Databases
        private IDatabaseService<DocumentFolder> database;
        private IConfigurationBuilder configurationBuilder;
        private DatabasesManager databasesManager = DatabasesManager.Manager;
        #endregion

        #region Services
        private ILogger<ApplicationError> exceptionLogger;
        #endregion

        #region Factories
        private IFactoriesManager factoriesManager = FactoriesManager.FactoriesManager.Instance;
        #endregion

        public FolderService()
        {
            databasesManager.MongoDBFactory.GetDatabase<DocumentFolder>().TryGetTarget(out database);

            configurationBuilder = new MongoConfigurationBuilder(database.Configuration);
            configurationBuilder.SetConfigurationFilePath(databaseConfigFile).SetAuthMechanism()
                    .SetUserName().SetPassword().SetDatabaseName().SetServerName().SetConnectionString();

            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);
            exceptionLogger.LogConfiguration = logConfiguration;
        }

        public async Task<IEnumerable<DocumentFolder>> GetFolders(long userID, long parentFolderID)
        {
            try
            {
                await database.Connect().ConfigureAwait(false);

                var filters = new FilterCollection(new IFilter[]
                {
                    new EqualityFilter<long>(typeof(DocumentFolder).GetBsonPropertyName("UserID"), userID),
                    new EqualityFilter<long>(typeof(DocumentFolder).GetBsonPropertyName("ParentFolderID"), parentFolderID)
                });

                return await database.Get(filters).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await exceptionLogger.Log(new ApplicationError(ex)).ConfigureAwait(false);
                throw new DatabaseException("The error occured while getting your folders");
            }
        }

        public async Task CreateFolder(long userID, long parentFolderID, string folderName)
        {
            try
            {
                await database.Connect().ConfigureAwait(false);

                await database.Insert(new DocumentFolder()
                {
                    CreationDate = DateTime.Now,
                    UserID = userID,
                    ParentFolderID = parentFolderID,
                    FolderName = folderName,
                    IsDeleted = false,
                    LastUpdateDate = DateTime.Now
                }).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await exceptionLogger.Log(new ApplicationError(ex)).ConfigureAwait(false);
                throw new DatabaseException("The error occured while creating new folder");
            }
        }

        public async Task DeleteFolder(long folderID)
        {
            try
            {
                await database.Connect().ConfigureAwait(false);

                var deletionFilter = new EqualityFilter<long>(typeof(DocumentFolder).GetBsonPropertyName("ID"),
                    folderID);

                await database.Delete(deletionFilter).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await exceptionLogger.Log(new ApplicationError(ex)).ConfigureAwait(false);
                throw new DatabaseException("The error occured while deleting the folder");
            }
        }

        public async Task RenameFolder(long folderID, string newFolderName)
        {
            try
            {
                await database.Connect().ConfigureAwait(false);

                var getFilter = new EqualityFilter<long>(typeof(DocumentFolder).GetBsonPropertyName("ID"), folderID);

                DocumentFolder documentFolder = (await database.Get(getFilter).ConfigureAwait(false)).FirstOrDefault();

                if (documentFolder == null)
                {
                    throw new ArgumentException();
                }

                documentFolder.FolderName = newFolderName;

                await database.Update(documentFolder).ConfigureAwait(false);
            }
            catch (Exception ex) when (ex.GetType() != typeof(ArgumentException))
            {
                await exceptionLogger.Log(new ApplicationError(ex)).ConfigureAwait(false);
                throw new DatabaseException("The error occured while renaming the folder");
            }
        }

        public async Task<DocumentFolder> GetFolderData(long folderID)
        {
            try
            {
                await database.Connect().ConfigureAwait(false);

                var getFilter = new EqualityFilter<long>(typeof(DocumentFolder).GetBsonPropertyName("ID"), folderID);

                return (await database.Get(getFilter).ConfigureAwait(false)).FirstOrDefault();
            }
            catch (Exception ex)
            {
                await exceptionLogger.Log(new ApplicationError(ex)).ConfigureAwait(false);
                throw new DatabaseException("The error occured while getting the folder data");
            }
        }
    }
}
