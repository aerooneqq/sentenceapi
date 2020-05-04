﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.IdentityModel.Tokens.Jwt;

using Application.Users.Interfaces;
 
using SharedLibrary.Loggers.Interfaces;
using SharedLibrary.FactoriesManager.Interfaces;

using DataAccessLayer.CommonInterfaces;
using DataAccessLayer.Configuration.Interfaces;
using DataAccessLayer.Configuration;
using DataAccessLayer.Filters;
using DataAccessLayer.Exceptions;
using DataAccessLayer.Hashes;
using DataAccessLayer.Filters.Base;
using DataAccessLayer.Filters.Interfaces;
using DataAccessLayer.DatabasesManager.Interfaces;

using Domain.Extensions;
using Domain.Logs;
using Domain.Logs.Configuration;
using Domain.Users;

using MongoDB.Bson;


namespace Application.Users.Services
{
    public class UserService : IUserService<UserInfo>
    {
        #region Static fields
        private static readonly string databaseConfigFile = "./configs/mongo_database_config.json";
        #endregion

        #region Databases
        private IDatabaseService<UserInfo> database;
        private IConfigurationBuilder configurationBuilder;
        #endregion

        #region Services
        private readonly ILogger<ApplicationError> exceptionLogger;
        #endregion

        private readonly LogConfiguration logConfiguration;


        #region Constructors
        public UserService(IFactoriesManager factoriesManager, IDatabaseManager databasesManager)
        {
            databasesManager.MongoDBFactory.GetDatabase<UserInfo>().TryGetTarget(out database);

            configurationBuilder = new MongoConfigurationBuilder(database.Configuration);
            configurationBuilder.SetConfigurationFilePath(databaseConfigFile).SetAuthMechanism()
                                .SetUserName().SetPassword().SetDatabaseName().SetServerName().SetConnectionString();

            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);

            logConfiguration = new LogConfiguration(this.GetType());
        }
        #endregion

        
        public async Task DeleteAsync(ObjectId id)
        {
            try
            {
                await database.Connect();

                var user = (await database.Get(new EqualityFilter<ObjectId>(typeof(UserInfo).GetBsonPropertyName("ID"), id))
                    .ConfigureAwait(false)).FirstOrDefault();

                if (user is null)
                {
                    throw new ArgumentException($"Can not delete the user with the ID ({id}), which does not exist");
                }

                user.IsAccountDeleted = true;

                await database.Update(user, new[] { "isAccountDeleted" });
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("The error occured while deleting account");
            }
        }

        public async Task<UserInfo> GetAsync(string token)
        {
            try
            {
                JwtSecurityToken jwtSecurityToken = new JwtSecurityTokenHandler().ReadToken(token) as JwtSecurityToken;
                string email = jwtSecurityToken.Claims.ToList().Find(c => c.Type == "Email").Value;

                await database.Connect().ConfigureAwait(false);

                var filter = new EqualityFilter<string>("email", email);
                return (await database.Get(filter)).FirstOrDefault();
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("Error occured while working with the database");
            }
        }

        public async Task<UserInfo> GetAsync(string email, string password)
        {
            try
            {
                await database.Connect();

                FilterBase emailFilter = new EqualityFilter<string>(
                    typeof(UserInfo).GetBsonPropertyName("Email"), email);
                FilterBase passwordFilter = new EqualityFilter<string>(
                    typeof(UserInfo).GetBsonPropertyName("Password"), password);

                var users = (await database.Get(emailFilter & passwordFilter)).ToList();

                if (users.Count != 1)
                {
                    return null;
                }

                return users[0];
            }
            catch (Exception ex) when (ex.GetType() != typeof(DatabaseException))
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("Error occured while working with the database");
            }
        }

        public async Task<UserInfo> GetAsync(ObjectId id)
        {
            try
            {
                await database.Connect().ConfigureAwait(false);

                return (await database.Get(new EqualityFilter<ObjectId>
                    (typeof(UserInfo).GetBsonPropertyName("ID"), id))).FirstOrDefault();
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("Error occured while working with the database");
            }
        }

        public async Task UpdateAsync(UserInfo user)
        {
            try
            {
                await database.Connect();
                await database.Update(user);
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("Error occured while updating record in the database");
            }
        }

        public async Task UpdateAsync(UserInfo user, IEnumerable<string> properties)
        {
            try
            {
                await database.Connect();
                await database.Update(user, properties);
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("The error occured while updating the user.");
            }
        }

        public async Task<ObjectId> CreateNewUserAsync(string email, string password)
        {
            try
            {
                UserInfo user = UserInfo.GetDefaultUser(email, password.GetMD5Hash());

                await database.Connect().ConfigureAwait(false);
                await database.Insert(user).ConfigureAwait(false);

                return user.ID;
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("Error occured when inserting user in the database");
            }
        }

        public async Task<IEnumerable<UserInfo>> FindUsersWithLoginAsync(string login)
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
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("Error occured when searching for users");
            }
        }

        /// <summary>
        /// Checks if user with a given email exists in the database.   
        /// </summary>
        public async Task<bool> DoesUserExistAsync(string email)
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
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("Error occured while checking the user");
            }
        }
    }
}
