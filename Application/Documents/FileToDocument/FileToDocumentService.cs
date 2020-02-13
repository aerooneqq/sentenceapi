using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

using DataAccessLayer.CommonInterfaces;
using DataAccessLayer.Configuration;
using DataAccessLayer.Configuration.Interfaces;
using DataAccessLayer.DatabasesManager.Interfaces;
using DataAccessLayer.Filters;
using DataAccessLayer.Filters.Base;
using DataAccessLayer.Exceptions;

using DocumentsAPI.Features.FileToDocument.Interfaces;

using SharedLibrary.FactoriesManager.Interfaces;
using SharedLibrary.Loggers.Interfaces;

using Domain.Logs;
using Domain.Logs.Configuration;

using MongoDB.Bson;


namespace DocumentsAPI.Features.FileToDocument.Services
{
    public class FileToDocumentService : IFileToDocumentService
    {
        private static readonly string databaseConfigFile = "./configs/mongo_database_config.json";
        
        #region Databases
        private readonly IDatabaseService<Models.FileToDocument> database;
        #endregion

        #region Services
        private readonly ILogger<ApplicationError> exceptionLogger;
        #endregion

        private readonly LogConfiguration logConfiguration;


        public FileToDocumentService(IFactoriesManager factoriesManager, IDatabaseManager databaseManager)
        {
            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);

            databaseManager.MongoDBFactory.GetDatabase<Models.FileToDocument>().TryGetTarget(out database);
            
            IConfigurationBuilder configurationBuilder = new MongoConfigurationBuilder(database.Configuration);
            configurationBuilder.SetConfigurationFilePath(databaseConfigFile).SetUserName().SetPassword()
                .SetAuthMechanism().SetDatabaseName().SetServerName().SetConnectionString();
            
            logConfiguration = new LogConfiguration(GetType());
        }


        public async Task AssociateFileWithDocumentAsync(ObjectId fileID, ObjectId documentID)
        {
            try
            {
                await database.Connect().ConfigureAwait(false);

                FilterBase fileIDGetFilter = new EqualityFilter<ObjectId>("FileID", fileID);

                var fileToDoc = (await database.Get(fileIDGetFilter).
                    ConfigureAwait(false)).FirstOrDefault();

                if (fileToDoc is null)
                {
                    fileToDoc = new Models.FileToDocument(fileID, documentID);
                    await database.Insert(fileToDoc).ConfigureAwait(false);
                }
                else
                {
                    if (fileToDoc.DocumentID == documentID)
                    {
                        return;
                    }

                    fileToDoc.DocumentID = documentID;

                    await database.Update(fileToDoc).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("The error occured while associating file with document", ex);
            }
        }

        public async Task<ObjectId> GetDocumentIDAsync(ObjectId fileID)
        {
            try
            {
                await database.Connect().ConfigureAwait(false);

                FilterBase getFilter = new EqualityFilter<ObjectId>("fileID", fileID);

                var fileToDoc = (await database.Get(getFilter).ConfigureAwait(false)).FirstOrDefault();

                if (fileToDoc is null)
                {
                    throw new KeyNotFoundException($"The file with id {fileID} is not yet associated with" +
                        "any document");
                }

                return fileToDoc.DocumentID;
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("The error occured while getting the associated document", ex);
            }
        }

        public async Task<Models.FileToDocument> GetFileToDocumentModelAsync(ObjectId fileID) 
        {
            try
            {
                await database.Connect().ConfigureAwait(false);

                FilterBase getFilter = new EqualityFilter<ObjectId>("fileID", fileID);

                return (await database.Get(getFilter).ConfigureAwait(false)).FirstOrDefault();
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("Error ocured while getting the file-document info");
            }
        }
    }
}