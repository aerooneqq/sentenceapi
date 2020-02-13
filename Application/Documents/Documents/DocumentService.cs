using System;
using System.Linq;
using System.Threading.Tasks;

using DataAccessLayer.CommonInterfaces;
using DataAccessLayer.Configuration;
using DataAccessLayer.Configuration.Interfaces;
using DataAccessLayer.DatabasesManager.Interfaces;
using DataAccessLayer.Exceptions;
using DataAccessLayer.Filters;

using DocumentsAPI.Features.Documents.Interfaces;

using Domain.Date;
using Domain.Logs;
using Domain.Logs.Configuration;
using Domain.Models.Document;

using SharedLibrary.FactoriesManager.Interfaces;
using SharedLibrary.Loggers.Interfaces;

using MongoDB.Bson;


namespace DocumentsAPI.Features.Documents.Services
{
    public class DocumentService : IDocumentService
    {
        private const string databaseConfigFile = "./configs/mongo_database_config.json";

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
            factoriesManager.GetService<IDateService>().TryGetTarget(out dateService);
            
            databaseManager.MongoDBFactory.GetDatabase<Document>().TryGetTarget(out database);
            
            IConfigurationBuilder configurationBuilder = new MongoConfigurationBuilder(database.Configuration);
            configurationBuilder.SetConfigurationFilePath(databaseConfigFile).SetUserName().SetPassword()
                .SetAuthMechanism().SetDatabaseName().SetServerName().SetConnectionString();
            
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
                                                 documentType, dateService.Now);

                await database.Insert(document).ConfigureAwait(false);

                return documentID;
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("The error occured while creating new document");
            }
        }

        public async Task<Document> GetDocumentByID(ObjectId documentID)
        {
            try
            {
                await database.Connect().ConfigureAwait(false);
                
                return (await database.Get(new EqualityFilter<ObjectId>("_id", documentID)).
                    ConfigureAwait(false)).FirstOrDefault();
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("The error occured while getting the document");
            }
        }

        public async Task DeleteDocumentByID(ObjectId documentID)
        {
            try
            {
                await database.Connect().ConfigureAwait(false);
                await database.Delete(new EqualityFilter<ObjectId>("_id", documentID)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("The error occured while deleting the document");
            }
        }
    }
}