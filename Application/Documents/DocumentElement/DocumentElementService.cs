using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Documents.DocumentElement.Interface;

using DataAccessLayer.CommonInterfaces;
using DataAccessLayer.Configuration;
using DataAccessLayer.Configuration.Interfaces;
using DataAccessLayer.DatabasesManager.Interfaces;
using DataAccessLayer.Exceptions;
using DataAccessLayer.Filters;

using Domain.DocumentElements;
using Domain.DocumentElements.Dto;
using Domain.Logs;
using Domain.Logs.Configuration;

using MongoDB.Bson;

using SharedLibrary.FactoriesManager.Interfaces;
using SharedLibrary.Loggers.Interfaces;


namespace Application.Documents.DocumentElement
{
    public class DocumentElementService : IDocumentElementService
    {
        private const string databaseConfigFile = "./configs/mongo_database_config.json";

        private readonly IDatabaseService<DocumentElementWrapper> database;

        private readonly ILogger<ApplicationError> exceptionLogger;

        private readonly LogConfiguration logConfiguration;


        public DocumentElementService(IFactoriesManager factoriesManager, IDatabaseManager databaseManager) 
        {
            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);

            databaseManager.MongoDBFactory.GetDatabase<DocumentElementWrapper>().TryGetTarget(out database);
            
            IConfigurationBuilder configurationBuilder = new MongoConfigurationBuilder(database.Configuration);
            configurationBuilder.SetConfigurationFilePath(databaseConfigFile).SetAuthMechanism()
                                .SetUserName().SetPassword().SetDatabaseName().SetServerName().SetConnectionString();

            database.CreateCollection();

            logConfiguration = new LogConfiguration(GetType());
        }


        public async Task<IEnumerable<DocumentElementDto>> GetDocumentElements(ObjectId parentItemID)
        {
            try
            {
                var elementWrapper = (await database.Get(new EqualityFilter<ObjectId>("parentItemID", parentItemID))
                    .ConfigureAwait(false)).FirstOrDefault();

                if (elementWrapper is null)
                {
                    return null;
                }

                return null;
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("The erorr occured while getting the document");
            }
        }
    }
}