using DataAccessLayer.CommonInterfaces;
using DataAccessLayer.Configuration;
using DataAccessLayer.Configuration.Interfaces;
using DataAccessLayer.DatabasesManager;
using DataAccessLayer.Exceptions;
using DataAccessLayer.Filters;
using DataAccessLayer.Filters.Base;

using SharedLibrary.Loggers.Interfaces;
using SharedLibrary.Loggers.Models;
using SentenceAPI.Extensions;
using SentenceAPI.Features.Workplace.DocumentsStorage.Interfaces;
using SentenceAPI.Features.Workplace.DocumentsStorage.Models;
using SharedLibrary.Loggers.Configuration;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SharedLibrary.FactoriesManager.Interfaces;
using SharedLibrary.FactoriesManager;
using DataAccessLayer.DatabasesManager.Interfaces;
using MongoDB.Bson;

namespace SentenceAPI.Features.Workplace.DocumentsStorage.Services
{
    public class FolderService : IFolderService
    {
        #region Static fields
        private static readonly string databaseConfigFile = "mongo_database_config.json";
        #endregion

        #region Databases
        private IDatabaseService<DocumentFolder> database;
        private IConfigurationBuilder configurationBuilder;
        #endregion

        #region Services
        private readonly ILogger<ApplicationError> exceptionLogger;
        #endregion

        private readonly LogConfiguration logConfiguration;

        public FolderService(IFactoriesManager factoriesManager, IDatabaseManager databasesManager)
        {
            databasesManager.MongoDBFactory.GetDatabase<DocumentFolder>().TryGetTarget(out database);

            configurationBuilder = new MongoConfigurationBuilder(database.Configuration);
            configurationBuilder.SetConfigurationFilePath(databaseConfigFile).SetAuthMechanism()
                    .SetUserName().SetPassword().SetDatabaseName().SetServerName().SetConnectionString();

            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);

            logConfiguration = new LogConfiguration(GetType());
        }

        public async Task<IEnumerable<DocumentFolder>> GetFolders(ObjectId userID, ObjectId parentFolderID)
        {
            try
            {
                await database.Connect().ConfigureAwait(false);

                FilterBase userIDFilter = new EqualityFilter<ObjectId>(typeof(DocumentFolder).GetBsonPropertyName("UserID"), userID);
                FilterBase parentFolderFilter = new EqualityFilter<ObjectId>(typeof(DocumentFolder).GetBsonPropertyName("ParentFolderID"), parentFolderID);

                return await database.Get(userIDFilter & parentFolderFilter).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("The error occured while getting your folders");
            }
        }

        public async Task<ObjectId> CreateFolder(ObjectId userID, ObjectId parentFolderID, string folderName)
        {
            try
            {
                ObjectId newFolderID = ObjectId.GenerateNewId();
                DocumentFolder documentFolder = new DocumentFolder()
                {
                    ID = newFolderID,
                    CreationDate = DateTime.Now,
                    UserID = userID,
                    ParentFolderID = parentFolderID,
                    FolderName = folderName,
                    IsDeleted = false,
                    LastUpdateDate = DateTime.Now
                };

                await database.Connect().ConfigureAwait(false);

                await database.Insert(documentFolder).ConfigureAwait(false);

                return newFolderID;
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("The error occured while creating new folder");
            }
        }

        public async Task Update(DocumentFolder folder)
        {
            try
            {
                await database.Connect().ConfigureAwait(false);

                await database.Update(folder);
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("The error happened while updating the folder");
            }
        }

        public async Task DeleteFolder(ObjectId folderID)
        {
            try
            {
                await database.Connect().ConfigureAwait(false);

                var deletionFilter = new EqualityFilter<ObjectId>(typeof(DocumentFolder).GetBsonPropertyName("ID"),
                    folderID);

                await database.Delete(deletionFilter).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("The error occured while deleting the folder");
            }
        }

        public async Task RenameFolder(ObjectId folderID, string newFolderName)
        {
            try
            {
                await database.Connect().ConfigureAwait(false);

                var getFilter = new EqualityFilter<ObjectId>(typeof(DocumentFolder).GetBsonPropertyName("ID"), folderID);

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
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("The error occured while renaming the folder");
            }
        }

        public async Task<DocumentFolder> GetFolderData(ObjectId folderID)
        {
            try
            {
                await database.Connect().ConfigureAwait(false);

                var getFilter = new EqualityFilter<ObjectId>(typeof(DocumentFolder).GetBsonPropertyName("ID"), folderID);

                return (await database.Get(getFilter).ConfigureAwait(false)).FirstOrDefault();
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("The error occured while getting the folder data");
            }
        }

        public async Task<IEnumerable<DocumentFolder>> GetFolders(ObjectId userID, string searchQuery)
        {
            try
            {
                await database.Connect().ConfigureAwait(false);

                FilterBase idFilter = new EqualityFilter<ObjectId>(typeof(DocumentFolder).GetBsonPropertyName("ID"), userID);
                FilterBase folderNameFilter = new RegexFilter(typeof(DocumentFolder).GetBsonPropertyName("FolderName"), $"/{searchQuery}/");

                return await database.Get(idFilter & folderNameFilter).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("The error occured while searching for folders");
            }
        }
    }
}
