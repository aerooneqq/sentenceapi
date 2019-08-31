using DataAccessLayer.CommonInterfaces;
using DataAccessLayer.Configuration;
using DataAccessLayer.Configuration.Interfaces;
using DataAccessLayer.DatabasesManager;
using DataAccessLayer.Exceptions;
using DataAccessLayer.Filters;
using DataAccessLayer.Filters.Interfaces;
using Microsoft.AspNetCore.Http;
using SentenceAPI.Extensions;
using SentenceAPI.Features.Authentication.Interfaces;
using SentenceAPI.Features.Codes.Interfaces;
using SentenceAPI.Features.Codes.Models;
using SentenceAPI.Features.Loggers.Interfaces;
using SentenceAPI.Features.Loggers.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentenceAPI.Features.Codes.Services
{
    class CodesService : ICodesService
    {
        #region Static fields
        private static Random random = new Random();
        private static int codeLength = 6;

        private static readonly LogConfiguration logConfiguration = new LogConfiguration()
        {
            ControllerName = string.Empty,
            ServiceName = "CodesService"
        };
        private static readonly string databaseConfigFile = "mongo_database_config.json";
        #endregion

        #region Databases
        private IDatabaseService<ActivationCode> database;
        private IConfigurationBuilder configurationBuilder;
        #endregion

        #region Services
        private ILogger<ApplicationError> exceptionLogger;
        private ITokenService tokenService;
        #endregion

        #region Factories
        private FactoriesManager.FactoriesManager factoriesManager =
            FactoriesManager.FactoriesManager.Instance;

        private ITokenServiceFactory tokenServiceFactory;
        private ILoggerFactory loggerFactory;
        #endregion

        public CodesService()
        {
            DatabasesManager.Manager.MongoDBFactory.GetDatabase<ActivationCode>().TryGetTarget(out database);

            configurationBuilder = new MongoConfigurationBuilder(database.Configuration);
            configurationBuilder.SetConfigurationFilePath(databaseConfigFile).SetAuthMechanism()
                                .SetUserName().SetPassword().SetDatabaseName().SetServerName().SetConnectionString();

            loggerFactory = (ILoggerFactory)factoriesManager[typeof(ILoggerFactory)];
            tokenServiceFactory = (ITokenServiceFactory)factoriesManager[typeof(ITokenServiceFactory)];

            exceptionLogger = loggerFactory.GetExceptionLogger();
            exceptionLogger.LogConfiguration = logConfiguration;

            tokenService = tokenServiceFactory.GetService();
        }

        public ActivationCode CreateActivationCode(long userID)
        {
            ActivationCode activationCode = GetActivationCode(userID);

            return activationCode;
        }

        private ActivationCode GetActivationCode(long userID)
        {
            return new ActivationCode()
            {
                Code = CreateCode(),
                CreationDate = DateTime.UtcNow,
                UsageDate = null,
                Used = false,
                UserID = userID,
            };
        }

        private string CreateCode()
        {
            string code = string.Empty;

            for (int i = 0; i < codeLength; i++)
            {
                code += (char)random.Next('A', 'Z' + 1);
            }

            return code;
        }

        /// <summary>
        /// Inserts the given activation code in the database.
        /// </summary
        /// <exception cref="DatabaseException">
        /// Fires when error happens while working with the database.
        /// </exception>
        public async Task InsertCodeInDatabase(ActivationCode activationCode)
        {
            try
            {
                await database.Connect();

                IFilter filter = new EqualityFilter<long>(typeof(ActivationCode).GetBsonPropertyName("UserID"),
                    activationCode.ID);

                await database.Delete(filter);
                await database.Insert(activationCode);
            }
            catch (Exception ex)
            {
                await exceptionLogger.Log(new ApplicationError(ex));
                throw new DatabaseException("The error happened inserting the activation code");
            }
        }

        public async Task ActivateCode(string code)
        {
            try
            {
                IFilter codeFilter = new EqualityFilter<string>(typeof(ActivationCode).GetBsonPropertyName("Code"), code);
                IFilter usedFilter = new EqualityFilter<bool>(typeof(ActivationCode).GetBsonPropertyName("Used"), false);

                IFilterCollection filterCollection = new FilterCollection(new[]
                {
                    codeFilter, usedFilter
                });

                await database.Connect();
                ActivationCode activationCode = (await database.Get(filterCollection).ConfigureAwait(false)).FirstOrDefault();
                
                if (activationCode == null)
                {
                    throw new DatabaseException("Such a code does not exist");
                }

                activationCode.Used = true;
                activationCode.UsageDate = DateTime.UtcNow;

                await database.Update(activationCode, new[] { "Used", "UsageDate" });
            }
            catch (Exception ex) when (ex.GetType() != typeof(DatabaseException))
            {
                await exceptionLogger.Log(new ApplicationError(ex));
                throw new DatabaseException("THe error happened while activating the code");
            }
        }
    }
}
