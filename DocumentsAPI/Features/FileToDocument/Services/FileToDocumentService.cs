using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

using DataAccessLayer.CommonInterfaces;
using DataAccessLayer.DatabasesManager.Interfaces;
using DataAccessLayer.Filters;
using DataAccessLayer.Filters.Base;

using DocumentsAPI.Features.FileToDocument.Interfaces;

using SharedLibrary.FactoriesManager.Interfaces;
using SharedLibrary.Loggers.Interfaces;
using SharedLibrary.Loggers.Models;
using SharedLibrary.Loggers.Configuration;
using DataAccessLayer.Exceptions;
using MongoDB.Bson;

namespace DocumentsAPI.Features.FileToDocument.Services
{
    public class FileToDocumentService : IFileToDocumentService
    {
        #region Databases
        private IDatabaseService<Models.FileToDocument> database;
        #endregion

        #region Services
        private ILogger<ApplicationError> exceptionLogger;
        #endregion

        private readonly LogConfiguration logConfiguration;


        public FileToDocumentService(IFactoriesManager factoriesManager, IDatabaseManager databaseManager)
        {
            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);
            logConfiguration = new LogConfiguration(GetType());

            databaseManager.MongoDBFactory.GetDatabase<Models.FileToDocument>().TryGetTarget(out database);
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