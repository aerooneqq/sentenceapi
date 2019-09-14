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
    public class FileService : IFileService
    {
        #region Static fields
        private static readonly string databaseConfigFile = "mongo_database_config.json";
        private static readonly LogConfiguration logConfiguration = new LogConfiguration()
        {
            ServiceName = typeof(FileService).Name,
            ControllerName = string.Empty
        };
        #endregion

        #region Databases
        private readonly IDatabaseService<DocumentFile> database;
        private readonly IConfigurationBuilder configurationBuilder;
        private readonly DatabasesManager databasesManager = DatabasesManager.Manager;
        #endregion

        #region Services
        private readonly ILogger<ApplicationError> exceptionLogger;
        #endregion

        #region Factories
        private readonly IFactoriesManager factoriesManager = FactoriesManager.FactoriesManager.Instance;
        #endregion

        public FileService()
        {
            databasesManager.MongoDBFactory.GetDatabase<DocumentFile>().TryGetTarget(out database);

            configurationBuilder = new MongoConfigurationBuilder(database.Configuration);
            configurationBuilder.SetConfigurationFilePath(databaseConfigFile).SetAuthMechanism()
                    .SetUserName().SetPassword().SetDatabaseName().SetServerName().SetConnectionString();

            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);
        }

        /// <summary>
        /// Gets all files from the parent folder, for a given user
        /// </summary>
        public async Task<IEnumerable<DocumentFile>> GetFiles(long userID, long parentFolderID)
        {
            try
            {
                await database.Connect().ConfigureAwait(false);
                var filters = new FilterCollection(new IFilter[]
                {
                    new EqualityFilter<long>(typeof(DocumentFile).GetBsonPropertyName("UserID"), userID),
                    new EqualityFilter<long>(typeof(DocumentFile).GetBsonPropertyName("ParentFolderID"), parentFolderID)
                });

                return await database.Get(filters).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await exceptionLogger.Log(new ApplicationError(ex)).ConfigureAwait(false);
                throw new DatabaseException("The error occured while getting the files");
            }
        }

        public async Task CreateNewFile(long userID, long parentFolderID, string fileName)
        {
            try
            {
                await database.Connect().ConfigureAwait(false);

                await database.Insert(new DocumentFile()
                {
                    CreationDate = DateTime.Now,
                    FileName = fileName,
                    LastUpdateDate = DateTime.Now,
                    ParentFolderID = parentFolderID,
                    UserID = userID
                }).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await exceptionLogger.Log(new ApplicationError(ex)).ConfigureAwait(false);
                throw new DatabaseException("The error occured while creating new files");
            }
        }
    }
}
