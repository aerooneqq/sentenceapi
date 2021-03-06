﻿using DataAccessLayer.CommonInterfaces;
using DataAccessLayer.Configuration;
using DataAccessLayer.Configuration.Interfaces;
using DataAccessLayer.Exceptions;
using DataAccessLayer.Filters;
using DataAccessLayer.Filters.Interfaces;
using DataAccessLayer.Filters.Base;
using DataAccessLayer.DatabasesManager.Interfaces;

using Application.Codes.Interfaces;
using Application.Tokens.Interfaces;

using SharedLibrary.Loggers.Interfaces;
using SharedLibrary.FactoriesManager.Interfaces;

using System;
using System.Linq;
using System.Threading.Tasks;

using Domain.Codes;
using Domain.Extensions;
using Domain.Logs;
using Domain.Logs.Configuration;

using MongoDB.Bson;


namespace Application.Codes.Services
{
    class CodesService : ICodesService
    {
        #region Static fields
        private static Random random = new Random();
        private static int codeLength = 6;
        private static readonly string databaseConfigFile = "./configs/mongo_database_config.json";
        #endregion


        #region Databases
        private readonly IDatabaseService<ActivationCode> database;
        #endregion


        #region Services
        private ILogger<ApplicationError> exceptionLogger;
        private ITokenService tokenService;
        #endregion

        private readonly LogConfiguration logConfiguration;
    

        public CodesService(IFactoriesManager factoriesManager, IDatabaseManager databaseManager)
        {
            databaseManager.MongoDBFactory.GetDatabase<ActivationCode>().TryGetTarget(out database);

            IConfigurationBuilder configurationBuilder = new MongoConfigurationBuilder(database.Configuration);
            configurationBuilder.SetConfigurationFilePath(databaseConfigFile).SetAuthMechanism()
                                .SetUserName().SetPassword().SetDatabaseName().SetServerName().SetConnectionString();

            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);
            factoriesManager.GetService<ITokenService>().TryGetTarget(out tokenService);

            logConfiguration = new LogConfiguration(this.GetType());
        }


        public ActivationCode CreateActivationCode(ObjectId userID)
        {
            ActivationCode activationCode = GetActivationCode(userID);

            return activationCode;
        }

        private ActivationCode GetActivationCode(ObjectId userID)
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
        public async Task InsertCodeInDatabaseAsync(ActivationCode activationCode)
        {
            try
            {
                await database.Connect().ConfigureAwait(false);

                IFilter filter = new EqualityFilter<ObjectId>(typeof(ActivationCode).GetBsonPropertyName("UserID"),
                    activationCode.ID);

                await database.Delete(filter).ConfigureAwait(false);
                await database.Insert(activationCode).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("The error happened inserting the activation code");
            }
        }

        public async Task ActivateCodeAsync(string code)
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
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("THe error happened while activating the code");
            }
        }
    }
}
