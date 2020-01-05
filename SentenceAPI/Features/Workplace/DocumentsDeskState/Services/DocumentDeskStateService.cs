using DataAccessLayer.CommonInterfaces;
using DataAccessLayer.Configuration;
using DataAccessLayer.Configuration.Interfaces;
using DataAccessLayer.DatabasesManager;
using DataAccessLayer.Exceptions;
using DataAccessLayer.Filters;
using DataAccessLayer.Filters.Interfaces;
using SharedLibrary.Loggers.Interfaces;
using SharedLibrary.Loggers.Models;
using SentenceAPI.Extensions;
using SentenceAPI.Features.Authentication.Interfaces;
using SentenceAPI.Features.Workplace.DocumentsDeskState.Interfaces;
using SentenceAPI.Features.Workplace.DocumentsDeskState.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SharedLibrary.FactoriesManager.Interfaces;
using SharedLibrary.FactoriesManager;
using SharedLibrary.Loggers.Configuration;
using MongoDB.Bson;
using DataAccessLayer.DatabasesManager.Interfaces;

namespace SentenceAPI.Features.Workplace.DocumentsDeskState.Services
{
    public class DocumentDeskStateService : IDocumentDeskStateService
    {
        private static readonly string databaseConfigFileName = "mongo_database_config.json";

        #region Databases
        private IDatabaseService<DocumentDeskState> database;
        private IConfigurationBuilder configurationBuilder;
        #endregion

        #region Services
        private ILogger<ApplicationError> exceptionLogger;
        private ITokenService tokenService;
        #endregion


        public DocumentDeskStateService(IFactoriesManager factoriesManager, IDatabaseManager databaseManager)
        {
            databaseManager.MongoDBFactory.GetDatabase<DocumentDeskState>().
                TryGetTarget(out database);

            configurationBuilder = new MongoConfigurationBuilder(database.Configuration);
            configurationBuilder.SetConfigurationFilePath(databaseConfigFileName).SetAuthMechanism()
                                .SetUserName().SetPassword().SetDatabaseName().SetServerName().SetConnectionString();

            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);
            exceptionLogger.LogConfiguration = new LogConfiguration(typeof(DocumentDeskStateService));

            factoriesManager.GetService<ITokenService>().TryGetTarget(out tokenService);
        }

        public async Task CreateDeskStateAsync(ObjectId userID)
        {
            try
            {
                await database.Connect().ConfigureAwait(false);

                await database.Insert(new DocumentDeskState()
                {
                    DocumentTopBarInfos = new List<DocumentTopBarInfo>(),
                    OpenedDocumentID = ObjectId.Empty,
                    UserID = userID
                }).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error);
                throw new DatabaseException("The error occured while creating new document desk state");
            }
        }

        public async Task DeleteDeskStateAsync(ObjectId userID)
        {
            try
            {
                await database.Connect().ConfigureAwait(false);

                IFilter deletionFilter = new EqualityFilter<ObjectId>("userID", userID);
                await database.Delete(deletionFilter).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error);
                throw new DatabaseException("The error occured while creating new document desk state");
            }
        }

        public async Task<DocumentDeskState> GetDeskStateAsync(string token)
        {
            try
            {
                return await GetDeskStateAsync(ObjectId.Parse(tokenService.GetTokenClaim("ID", token))).ConfigureAwait(false);
            }
            catch (Exception ex) when (ex.GetType() != typeof(DatabaseException))
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error);
                throw new DatabaseException("Error occured while getting the desk state");
            }
        }

        public async Task<DocumentDeskState> GetDeskStateAsync(ObjectId userID)
        {
            try
            {
                await database.Connect().ConfigureAwait(false);

                var getFilter = new EqualityFilter<ObjectId>(typeof(DocumentDeskState).GetBsonPropertyName("ID"), userID);
                return (await database.Get(getFilter).ConfigureAwait(false)).FirstOrDefault();
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error);
                throw new DatabaseException("The error occured while getting the desk state", ex);
            }
        }

        public async Task UpdateDeskStateAsync(DocumentDeskState documentDeskState)
        {
            try
            {
                await database.Connect();
                await database.Update(documentDeskState);
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error);
                throw new DatabaseException("The error occured while updating the desk state", ex);
            }
        }

        public async Task UpdateAsync(DocumentDeskState documentDeskState, IEnumerable<string> properties)
        {
            try
            {
                await database.Connect().ConfigureAwait(false);
                await database.Update(documentDeskState, properties).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error);
                throw new DatabaseException("The error occured while updating the desk state");
            }
        }
    }
}
