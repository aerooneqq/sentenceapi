using System;
using System.Threading.Tasks;

using DataAccessLayer.CommonInterfaces;
using DataAccessLayer.DatabasesManager.Interfaces;
using DocumentsAPI.Features.Documents.Interfaces;
using DocumentsAPI.Models.Document;

using SharedLibrary.FactoriesManager.Interfaces;
using SharedLibrary.Loggers.Interfaces;
using SharedLibrary.Loggers.Models;
using SharedLibrary.Date.Interfaces;
using SharedLibrary.Loggers.Configuration;

using MongoDB.Bson;

using DataAccessLayer.Exceptions;


namespace DocumentsAPI.Features.Documents.Services
{
    public class DocumentService : IDocumentService
    {
        #region Databases
        private readonly IDatabaseService<Document> database;
        #endregion

        #region Services
        private readonly ILogger<ApplicationError> exceptionLogger;
        private readonly IDateService dateService;
        #endregion

        private readonly LogConfiguration logConfiguration;


        public DocumentService(IFactoriesManager factoriesManager, IDatabaseManager databaseManager)
        {
            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);

            databaseManager.MongoDBFactory.GetDatabase<Document>().TryGetTarget(out database);

            logConfiguration = new LogConfiguration(GetType());
        }

        public async Task<ObjectId> CreateEmptyDocument(ObjectId userID, string name, 
                                                        DocumentType documentType)
        {
            try
            {
                await database.Connect().ConfigureAwait(false);
                ObjectId documentID = ObjectId.GenerateNewId();

                Document document = new Document(documentID, userID, name, 
                                                 documentType, dateService.GetCurrentDate());

                await database.Insert(document).ConfigureAwait(false);

                return documentID;
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("The error occured while creating new document");
            }
        }
    }
}