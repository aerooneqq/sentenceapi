using DataAccessLayer.CommonInterfaces;
using DataAccessLayer.Configuration;
using DataAccessLayer.Configuration.Interfaces;
using DataAccessLayer.DatabasesManager;
using DataAccessLayer.Exceptions;
using DataAccessLayer.Filters;
using DataAccessLayer.Filters.Interfaces;
using SentenceAPI.ApplicationFeatures.Loggers.Interfaces;
using SentenceAPI.ApplicationFeatures.Loggers.Models;
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

namespace SentenceAPI.Features.Workplace.DocumentsDeskState.Services
{
    public class DocumentDeskStateService : IDocumentDeskStateService
    {
        private static readonly string databaseConfigFileName = "mongo_database_config.json";
        private static readonly LogConfiguration logConfiguration = new LogConfiguration()
        {
            ControllerName = string.Empty,
            ServiceName = "DocumentDeskStateService"
        };

        #region Databases
        private IDatabaseService<DocumentDeskState> database;
        private IConfigurationBuilder configurationBuilder;
        private DatabasesManager databasesManager = DatabasesManager.Manager;
        #endregion

        #region Services
        private ILogger<ApplicationError> exceptionLogger;
        private ITokenService tokenService;
        #endregion

        #region Factories
        private readonly IFactoriesManager factoriesManager = 
            ManagersDictionary.Instance.GetManager(Startup.ApiName);
        #endregion

        public DocumentDeskStateService()
        {
            databasesManager.MongoDBFactory.GetDatabase<DocumentDeskState>().
                TryGetTarget(out database);

            configurationBuilder = new MongoConfigurationBuilder(database.Configuration);
            configurationBuilder.SetConfigurationFilePath(databaseConfigFileName).SetAuthMechanism()
                                .SetUserName().SetPassword().SetDatabaseName().SetServerName().SetConnectionString();

            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);
            exceptionLogger.LogConfiguration = logConfiguration;

            factoriesManager.GetService<ITokenService>().TryGetTarget(out tokenService);
        }

        public async Task CreateDeskState(long userID)
        {
            try
            {
                await database.Connect().ConfigureAwait(false);

                await database.Insert(new DocumentDeskState()
                {
                    DocumentTopBarInfos = new List<DocumentTopBarInfo>(),
                    OpenedDocumentID = -1,
                    UserID = userID
                }).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await exceptionLogger.Log(new ApplicationError(ex)).ConfigureAwait(false);
                throw new DatabaseException("The error occured while creating new document desk state");
            }
        }

        public async Task DeleteDeskState(long userID)
        {
            try
            {
                await database.Connect().ConfigureAwait(false);

                IFilter deletionFilter = new EqualityFilter<long>("userID", userID);
                await database.Delete(deletionFilter).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await exceptionLogger.Log(new ApplicationError(ex)).ConfigureAwait(false);
                throw new DatabaseException("The error occured while creating new document desk state");
            }
        }

        public async Task<DocumentDeskState> GetDeskState(string token)
        {
            try
            {
                return await GetDeskState(long.Parse(tokenService.GetTokenClaim("ID", token))).ConfigureAwait(false);
            }
            catch (Exception ex) when (ex.GetType() != typeof(DatabaseException))
            {
                await exceptionLogger.Log(new ApplicationError(ex)).ConfigureAwait(false);
                throw new DatabaseException("Error occured while getting the desk state");
            }
        }

        public async Task<DocumentDeskState> GetDeskState(long userID)
        {
            try
            {
                await database.Connect().ConfigureAwait(false);

                var getFilter = new EqualityFilter<long>(typeof(DocumentDeskState).GetBsonPropertyName("ID"), userID);
                return (await database.Get(getFilter).ConfigureAwait(false)).FirstOrDefault();
            }
            catch (Exception ex)
            {
                await exceptionLogger.Log(new ApplicationError(ex)).ConfigureAwait(false);
                throw new DatabaseException("The error occured while getting the desk state");
            }
        }

        public async Task UpdateDeskState(DocumentDeskState documentDeskState)
        {
            try
            {
                await database.Connect();
                await database.Update(documentDeskState);
            }
            catch (Exception ex)
            {
                await exceptionLogger.Log(new ApplicationError(ex)).ConfigureAwait(false);
                throw new DatabaseException("The error occured while updating the desk state");
            }
        }

        public async Task Update(DocumentDeskState documentDeskState, IEnumerable<string> properties)
        {
            try
            {
                await database.Connect().ConfigureAwait(false);
                await database.Update(documentDeskState, properties).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await exceptionLogger.Log(new ApplicationError(ex)).ConfigureAwait(false);
                throw new DatabaseException("The error occured while updating the desk state");
            }
        }
    }
}
