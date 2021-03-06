﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Tokens.Interfaces;
using DataAccessLayer.CommonInterfaces;
using DataAccessLayer.Configuration;
using DataAccessLayer.Configuration.Interfaces;
using DataAccessLayer.DatabasesManager.Interfaces;
using DataAccessLayer.Exceptions;
using DataAccessLayer.Filters;
using DataAccessLayer.Filters.Interfaces;
using Domain.Extensions;
using Domain.Logs;
using Domain.Logs.Configuration;
using Domain.Workplace.DocumentsDeskState;
using MongoDB.Bson;
using SentenceAPI.Features.Workplace.DocumentsDeskState.Interfaces;
using SharedLibrary.FactoriesManager.Interfaces;
using SharedLibrary.Loggers.Interfaces;

namespace Application.Workplace.DocumentDeskState
{
    public class DocumentDeskStateService : IDocumentDeskStateService
    {
        private static readonly string databaseConfigFileName = "./configs/mongo_database_config.json";

        #region Databases
        private readonly IDatabaseService<Domain.Workplace.DocumentsDeskState.DocumentDeskState> database;
        private readonly IConfigurationBuilder configurationBuilder;
        #endregion

        #region Services
        private readonly ILogger<ApplicationError> exceptionLogger;
        private readonly ITokenService tokenService;
        #endregion

        private readonly LogConfiguration logConfiguration;

        public DocumentDeskStateService(IFactoriesManager factoriesManager, IDatabaseManager databaseManager)
        {
            databaseManager.MongoDBFactory.GetDatabase<Domain.Workplace.DocumentsDeskState.DocumentDeskState>().
                TryGetTarget(out database);

            configurationBuilder = new MongoConfigurationBuilder(database.Configuration);
            configurationBuilder.SetConfigurationFilePath(databaseConfigFileName).SetAuthMechanism()
                                .SetUserName().SetPassword().SetDatabaseName().SetServerName().SetConnectionString();

            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);
            logConfiguration = new LogConfiguration(typeof(DocumentDeskStateService));

            factoriesManager.GetService<ITokenService>().TryGetTarget(out tokenService);
        }

        public async Task CreateDeskStateAsync(ObjectId userID)
        {
            try
            {
                await database.Connect().ConfigureAwait(false);

                await database.Insert(new Domain.Workplace.DocumentsDeskState.DocumentDeskState()
                {
                    DocumentTopBarInfos = new List<DocumentTopBarInfo>(),
                    OpenedDocumentID = ObjectId.Empty,
                    UserID = userID
                }).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
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
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("The error occured while creating new document desk state");
            }
        }

        public async Task<Domain.Workplace.DocumentsDeskState.DocumentDeskState> GetDeskStateAsync(string token)
        {
            try
            {
                return await GetDeskStateAsync(ObjectId.Parse(tokenService.GetTokenClaim(token, "ID"))).ConfigureAwait(false);
            }
            catch (Exception ex) when (ex.GetType() != typeof(DatabaseException))
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("Error occured while getting the desk state");
            }
        }

        public async Task<Domain.Workplace.DocumentsDeskState.DocumentDeskState> GetDeskStateAsync(ObjectId userID)
        {
            try
            {
                await database.Connect().ConfigureAwait(false);

                var getFilter = new EqualityFilter<ObjectId>(typeof(Domain.Workplace.DocumentsDeskState.DocumentDeskState).GetBsonPropertyName("UserID"), userID);
                return (await database.Get(getFilter).ConfigureAwait(false)).FirstOrDefault();
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("The error occured while getting the desk state", ex);
            }
        }

        public async Task UpdateDeskStateAsync(Domain.Workplace.DocumentsDeskState.DocumentDeskState documentDeskState)
        {
            try
            {
                await database.Connect();
                await database.Update(documentDeskState);
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("The error occured while updating the desk state", ex);
            }
        }

        public async Task UpdateAsync(Domain.Workplace.DocumentsDeskState.DocumentDeskState documentDeskState, IEnumerable<string> properties)
        {
            try
            {
                await database.Connect().ConfigureAwait(false);
                await database.Update(documentDeskState, properties).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("The error occured while updating the desk state");
            }
        }
    }
}
