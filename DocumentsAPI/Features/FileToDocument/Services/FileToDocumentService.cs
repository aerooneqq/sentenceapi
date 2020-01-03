using System;
using System.Threading.Tasks;
using System.Linq; 
using System.Collections.Generic;

using DataAccessLayer.CommonInterfaces;
using DataAccessLayer.DatabasesManager.Interfaces;
using DataAccessLayer.DatabasesManager;
using DataAccessLayer.Filters;
using DataAccessLayer.Filters.Base;

using DocumentsAPI.Features.FileToDocument.Interfaces;

using SharedLibrary.FactoriesManager.Interfaces;
using SharedLibrary.Loggers.Interfaces;
using SharedLibrary.Loggers.Models;
using SharedLibrary.FactoriesManager;
using SharedLibrary.Loggers.Configuration;
using DataAccessLayer.Exceptions;


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


        public FileToDocumentService(IFactoriesManager factoriesManager, IDatabaseManager databaseManager)
        {
            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);
            exceptionLogger.LogConfiguration = new LogConfiguration(GetType())
            {
                ClassName = this.GetType().FullName,
                ComponentType = ComponentType.Service
            };

            databaseManager.MongoDBFactory.GetDatabase<Models.FileToDocument>().TryGetTarget(out database);
        }


        public async Task AssociateFileWithDocumentAsync(long fileID, long documentID)
        {
            try
            {
                FilterBase fileIDGetFilter = new EqualityFilter<long>("FileID", fileID);

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
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error);
                throw new DatabaseException("The error occured while associating file with document", ex);
            }
        }

        public async Task<long> GetDocumentIDAsync(long fileID)
        {
            try
            {
                FilterBase getFilter = new EqualityFilter<long>("FileID", fileID);

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
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error);
                throw new DatabaseException("The error occured while getting the associated document", ex);
            }
        }
    }
}