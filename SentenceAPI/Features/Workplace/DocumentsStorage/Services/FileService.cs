using DataAccessLayer.CommonInterfaces;
using DataAccessLayer.Configuration;
using DataAccessLayer.Configuration.Interfaces;
using DataAccessLayer.DatabasesManager;
using DataAccessLayer.Exceptions;
using DataAccessLayer.Filters;
using DataAccessLayer.Filters.Base;
using DataAccessLayer.Filters.Interfaces;

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
    public class FileService : IFileService
    {
        #region Static fields
        private static readonly string databaseConfigFile = "mongo_database_config.json";
        #endregion

        #region Databases
        private readonly IDatabaseService<DocumentFile> database;
        private readonly IConfigurationBuilder configurationBuilder;
        #endregion

        #region Services
        private readonly ILogger<ApplicationError> exceptionLogger;
        #endregion

        public FileService(IFactoriesManager factoriesManager, IDatabaseManager databasesManager)
        {
            databasesManager.MongoDBFactory.GetDatabase<DocumentFile>().TryGetTarget(out database);

            configurationBuilder = new MongoConfigurationBuilder(database.Configuration);
            configurationBuilder.SetConfigurationFilePath(databaseConfigFile).SetAuthMechanism()
                    .SetUserName().SetPassword().SetDatabaseName().SetServerName().SetConnectionString();

            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);
            exceptionLogger.LogConfiguration = new LogConfiguration(this.GetType());
        }

        /// <summary>
        /// Gets all files from the parent folder, for a given user
        /// </summary>
        public async Task<IEnumerable<DocumentFile>> GetFilesAsync(ObjectId userID, ObjectId parentFolderID)
        {
            try
            {
                await database.Connect().ConfigureAwait(false);

                FilterBase userIDFilter = new EqualityFilter<ObjectId>(typeof(DocumentFile).GetBsonPropertyName(
                    "UserID"), userID);
                FilterBase parentFolderIDFilter = new EqualityFilter<ObjectId>(typeof(DocumentFile).GetBsonPropertyName(
                    "ParentFolderID"), parentFolderID);

                return await database.Get(userIDFilter & parentFolderIDFilter).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error);
                throw new DatabaseException("The error occured while getting the files");
            }
        }

        public async Task CreateNewFileAsync(ObjectId userID, ObjectId parentFolderID, string fileName)
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
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error);
                throw new DatabaseException("The error occured while creating new files", ex);
            }
        }

        public async Task DeleteFileAsync(ObjectId fileID)
        {
            try
            {
                await database.Connect().ConfigureAwait(false);

                var deletionFilter = new EqualityFilter<ObjectId>(typeof(DocumentFile).GetBsonPropertyName("ID"),
                    fileID);

                await database.Delete(deletionFilter).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error);
                throw new DatabaseException("The error occured while deleting the file", ex);
            }
        }

        public async Task<IEnumerable<DocumentFile>> GetFilesAsync(ObjectId userID, string searchQuery)
        {
            try
            {
                await database.Connect().ConfigureAwait(false);


                FilterBase idFilter = new EqualityFilter<ObjectId>(typeof(DocumentFile).
                    GetBsonPropertyName("ID"), userID);
                FilterBase fileNameFilter = new RegexFilter(typeof(DocumentFile).GetBsonPropertyName(
                    "FileName"), $"/{searchQuery}/");

                return await database.Get(idFilter & fileNameFilter).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error);
                throw new DatabaseException("The error occured while searching for the files", ex);
            }
        }

        public async Task<DocumentFile> GetFileAsync(ObjectId fileID)
        {
            try
            {
                await database.Connect().ConfigureAwait(false);
                IFilter getFilter = new EqualityFilter<ObjectId>(typeof(DocumentFile).
                    GetBsonPropertyName("ID"), fileID);
                
                return (await database.Get(getFilter).ConfigureAwait(false)).FirstOrDefault();
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error);
                throw new DatabaseException("The error occured while getting the file data", ex);
            }
        }

        public async Task UpdateAsync(DocumentFile documentFile)
        {
            try
            {
                await database.Connect().ConfigureAwait(false);
                await database.Update(documentFile).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error);
                throw new DatabaseException("The error occured while updating file", ex);
            }
        }

        public async Task RenameFileAsync(ObjectId fileID, string newFileName)
        {
            try
            {
                await database.Connect().ConfigureAwait(false);

                IFilter getFilter = new EqualityFilter<ObjectId>("ID", fileID);
                DocumentFile file = (await database.Get(getFilter).ConfigureAwait(false)).FirstOrDefault();

                if (!(file is DocumentFile))
                {
                    throw new ArgumentException("The file with such an id does not exist");
                }

                file.FileName = newFileName;

                await database.Update(file).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error);
                throw new DatabaseException("The error occured while renaming file", ex);
            }
        }
    }
}
