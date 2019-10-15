using DataAccessLayer.CommonInterfaces;
using DataAccessLayer.Configuration;
using DataAccessLayer.Configuration.Interfaces;
using DataAccessLayer.DatabasesManager;
using DataAccessLayer.Exceptions;
using DataAccessLayer.Filters;
using DataAccessLayer.Filters.Interfaces;

using SentenceAPI.Extensions;
using SentenceAPI.Features.Authentication.Interfaces;
using SentenceAPI.Features.Codes.Interfaces;
using SentenceAPI.Features.Codes.Models;
using SentenceAPI.ApplicationFeatures.Loggers.Interfaces;
using SentenceAPI.ApplicationFeatures.Loggers.Models;

using System;
using System.Linq;
using System.Threading.Tasks;
using DataAccessLayer.Filters.Base;

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
        #endregion

        public CodesService()
        {
            DatabasesManager.Manager.MongoDBFactory.GetDatabase<ActivationCode>().TryGetTarget(out database);

            configurationBuilder = new MongoConfigurationBuilder(database.Configuration);
            configurationBuilder.SetConfigurationFilePath(databaseConfigFile).SetAuthMechanism()
                                .SetUserName().SetPassword().SetDatabaseName().SetServerName().SetConnectionString();

            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);
            factoriesManager.GetService<ITokenService>().TryGetTarget(out tokenService);

            exceptionLogger.LogConfiguration = logConfiguration;
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
                await database.Connect().ConfigureAwait(false);

                IFilter filter = new EqualityFilter<long>(typeof(ActivationCode).GetBsonPropertyName("UserID"),
                    activationCode.ID);

                await database.Delete(filter).ConfigureAwait(false);
                await database.Insert(activationCode).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await exceptionLogger.Log(new ApplicationError(ex)).ConfigureAwait(false);
                throw new DatabaseException("The error happened inserting the activation code");
            }
        }

        public async Task ActivateCode(string code)
        {
            try
            {
                FilterBase codeFilter = new EqualityFilter<string>(typeof(ActivationCode)
                    .GetBsonPropertyName("Code"), code);
                FilterBase usedFilter = new EqualityFilter<bool>(typeof(ActivationCode)
                    .GetBsonPropertyName("Used"), false);

                await database.Connect().ConfigureAwait(false);
                ActivationCode activationCode = (await database.Get(codeFilter & usedFilter)
                    .ConfigureAwait(false)).FirstOrDefault();

                if (activationCode == null)
                {
                    throw new DatabaseException("Such a code does not exist");
                }

                activationCode.Used = true;
                activationCode.UsageDate = DateTime.UtcNow;

                await database.Update(activationCode, new[] { "Used", "UsageDate" }).ConfigureAwait(false);
            }
            catch (Exception ex) when (ex.GetType() != typeof(DatabaseException))
            {
                await exceptionLogger.Log(new ApplicationError(ex)).ConfigureAwait(false);
                throw new DatabaseException("THe error happened while activating the code");
            }
        }
    }
}
