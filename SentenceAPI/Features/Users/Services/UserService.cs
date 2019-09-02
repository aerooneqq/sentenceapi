﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using MongoDB.Bson.Serialization.Attributes;

using SentenceAPI.Features.Users.Interfaces;
using SentenceAPI.Features.Users.Models;
using SentenceAPI.Features.Loggers.Interfaces;
using SentenceAPI.Features.Loggers.Models;
using SentenceAPI.Extensions;

using DataAccessLayer.CommonInterfaces;
using DataAccessLayer.Configuration.Interfaces;
using DataAccessLayer.DatabasesManager;
using DataAccessLayer.Configuration;
using DataAccessLayer.Filters;
using DataAccessLayer.Exceptions;
using DataAccessLayer.Hashes;

namespace SentenceAPI.Features.Users.Services
{
    public class UserService : IUserService<UserInfo>
    {
        #region Static fields
        private static LogConfiguration LogConfiguration { get; } = new LogConfiguration()
        {
            ControllerName = string.Empty,
            ServiceName = "UserService"
        };
        private static readonly string databaseConfigFile = "mongo_database_config.json";
        #endregion

        #region Databases
        private IDatabaseService<UserInfo> database;
        private IConfigurationBuilder configurationBuilder;
        private DatabasesManager databasesManager = DatabasesManager.Manager;
        #endregion

        #region Services
        private readonly ILogger<ApplicationError> exceptionLogger;
        #endregion

        #region Factories
        private readonly FactoriesManager.FactoriesManager factoriesManager =
            FactoriesManager.FactoriesManager.Instance;
        private readonly ILoggerFactory loggerFactory;
        #endregion

        #region Constructors
        public UserService()
        {
            databasesManager.MongoDBFactory.GetDatabase<UserInfo>().TryGetTarget(out database);

            configurationBuilder = new MongoConfigurationBuilder(database.Configuration);
            configurationBuilder.SetConfigurationFilePath(databaseConfigFile).SetAuthMechanism()
                                .SetUserName().SetPassword().SetDatabaseName().SetServerName().SetConnectionString();

            loggerFactory = factoriesManager[typeof(ILoggerFactory)] as ILoggerFactory;

            exceptionLogger = loggerFactory.GetExceptionLogger();
            exceptionLogger.LogConfiguration = LogConfiguration;
        }
        #endregion

        public void Delete(long id)
        {
            throw new NotImplementedException();
        }

        public async Task<UserInfo> Get(string token)
        {
            try
            {
                JwtSecurityToken jwtSecurityToken = new JwtSecurityTokenHandler().ReadToken(token) as JwtSecurityToken;
                string email = jwtSecurityToken.Claims.ToList().Find(c => c.Type == "Email").Value;

                await database.Connect();

                var filter = new EqualityFilter<string>("email", email);
                return (await database.Get(filter)).FirstOrDefault();
            }
            catch (Exception ex)
            {
                await exceptionLogger.Log(new ApplicationError(ex.Message));
                throw new DatabaseException("Error occured while working with the database");
            }
        }

        public async Task<UserInfo> Get(string email, string password)
        {
            try
            {
                await database.Connect();

                IEnumerable<IFilter> filters = new[]
                {
                    new EqualityFilter<string>(typeof(UserInfo).GetBsonPropertyName("Email"), email),
                    new EqualityFilter<string>(typeof(UserInfo).GetBsonPropertyName("Password"), password)
                };

                var users = (await database.Get(new FilterCollection(filters))).ToList();

                if (users.Count != 1)
                {
                    return null;
                }

                return users[0];
            }
            catch (Exception ex) when (ex.GetType() != typeof(DatabaseException))
            {
                await exceptionLogger.Log(new ApplicationError(ex.Message));
                throw new DatabaseException("Error occured while working with the database");
            }
        }

        public async Task<UserInfo> Get(long id)
        {
            try
            {
                await database.Connect();

                return (await database.Get(new EqualityFilter<long>
                    (typeof(UserInfo).GetBsonPropertyName("ID"), id))).FirstOrDefault();
            }
            catch (Exception ex)
            {
                await exceptionLogger.Log(new ApplicationError(ex.Message));
                throw new DatabaseException("Error occured while working with the database");
            }
        }

        public async Task Update(UserInfo user)
        {
            try
            {
                await database.Connect();
                await database.Update(user);
            }
            catch (Exception ex)
            {
                await exceptionLogger.Log(new ApplicationError(ex.Message));
                throw new DatabaseException("Error occured while ipdating record in the database");
            }
        }

        public async Task Update(UserInfo user, IEnumerable<string> properties)
        {
            try
            {
                await database.Connect();
                await database.Update(user, properties);
            }
            catch (Exception ex)
            {
                await exceptionLogger.Log(new ApplicationError(ex));
                throw new DatabaseException("The error occured while updating the user.");
            }
        }

        public async Task<long> CreateNewUser(string email, string password)
        {
            try
            {
                UserInfo user = new UserInfo()
                {
                    Email = email,
                    Password = password.GetMD5Hash(),
                    IsAccountVerified = false,
                };

                await database.Connect();
                await database.Insert(user);

                return user.ID;
            }
            catch (Exception ex)
            {
                await exceptionLogger.Log(new ApplicationError(ex.Message));
                throw new DatabaseException("Error occured when inserting a user in the database");
            }
        }

        public async Task<IEnumerable<UserInfo>> FindUsersWithLogin(string login)
        {
            try
            {
                await database.Connect();

                string bsonPropertyName = typeof(UserInfo).GetBsonPropertyName("Login");
                var filter = new RegexFilter(bsonPropertyName, $"/{login}/");

                return await database.Get(filter);
            }
            catch (Exception ex)
            {
                await exceptionLogger.Log(new ApplicationError(ex.Message));
                throw new DatabaseException("Error occured when inserting a user in the database");
            }
        }

        /// <summary>
        /// Checks if user with a given email exists in the database.
        /// </summary>
        public async Task<bool> DoesUserExist(string email)
        {
            try
            {
                await database.Connect();

                string emailPropertyName = typeof(UserInfo).GetBsonPropertyName("Email");
                IFilter emailFilter = new EqualityFilter<string>(emailPropertyName, email);

                var users = (await database.Get(emailFilter).ConfigureAwait(false)).ToList();

                if (users.Count > 0)
                {
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                await exceptionLogger.Log(new ApplicationError(ex.Message));
                throw new DatabaseException("Error occured while checking the user");
            }
        }
    }
}
