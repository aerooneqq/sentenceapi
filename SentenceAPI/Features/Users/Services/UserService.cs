using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using System;

using MongoDB.Bson.Serialization.Attributes;

using SentenceAPI.Databases.MongoDB.Interfaces;
using SentenceAPI.Features.Users.Interfaces;
using SentenceAPI.Features.Users.Models;
using SentenceAPI.Databases.Exceptions;
using SentenceAPI.Features.Loggers.Interfaces;
using SentenceAPI.Features.Loggers.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SentenceAPI.Features.Users.Services
{
    public class UserService : IUserService<UserInfo>
    {
        public static LogConfiguration LogConfiguration { get; } = new LogConfiguration()
        {
            ControllerName = string.Empty,
            ServiceName = "UserService"
        };

        #region Services
        private readonly ILogger<ApplicationError> exceptionLogger;
        private readonly IMongoDBService<UserInfo> mongoDBService;
        #endregion

        #region Builders
        private readonly IMongoDBServiceBuilder<UserInfo> mongoDBServiceBuilder;
        #endregion

        #region Factories
  
        private readonly FactoriesManager.FactoriesManager factoriesManager =
            FactoriesManager.FactoriesManager.Instance;
        private readonly IMongoDBServiceFactory mongoDBServiceFactory;
        private readonly ILoggerFactory loggerFactory;
        #endregion

        #region Constructors
        public UserService()
        {
            mongoDBServiceFactory = factoriesManager[typeof(IMongoDBServiceFactory)].Factory
                as IMongoDBServiceFactory;
            loggerFactory = factoriesManager[typeof(ILoggerFactory)].Factory as ILoggerFactory;

            mongoDBService = mongoDBServiceFactory.GetService<UserInfo>();
            mongoDBServiceBuilder = mongoDBServiceFactory.GetBuilder(mongoDBService);

            mongoDBService = mongoDBServiceBuilder.AddConfigurationFile("database_config.json")
                .SetConnectionString()
                .SetDatabaseName("SentenceDatabase")
                .SetCollectionName()
                .Build();

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

                await mongoDBService.Connect();
                return mongoDBService.Get(new Dictionary<string, object>()
                {
                    { GetPropertyBsonElement("Email"), email }
                }).GetAwaiter().GetResult().FirstOrDefault();
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
                await mongoDBService.Connect();

                string emailPropertyName = GetPropertyBsonElement("Email");
                string passwordPropertyName = GetPropertyBsonElement("Password");

                var users = (await mongoDBService.Get(new Dictionary<string, object>()
                {
                    { emailPropertyName, email },
                    { passwordPropertyName, password }
                })).ToList();

                if (users.Count != 1)
                {
                    return null;
                }

                return users[0];
            }
            catch (Exception ex)
            {
                await exceptionLogger.Log(new ApplicationError(ex.Message));
                throw new DatabaseException("Error occured while working with the database");
            }
        }

        public async Task<UserInfo> Get(long id)
        {
            try
            {
                await mongoDBService.Connect();

                return await mongoDBService.Get(id);
            }
            catch (Exception ex)
            {
                await exceptionLogger.Log(new ApplicationError(ex.Message));
                throw new DatabaseException("Error occured while working with the database");
            }
        }

        private string GetPropertyBsonElement(string propertyName)
        {
            return typeof(UserInfo).GetProperty(propertyName).GetCustomAttribute<BsonElementAttribute>().
                ElementName;
        }

        public void Update(UserInfo user)
        {
            throw new NotImplementedException();
        }

        public async Task<long> CreateNewUser(string email, string password)
        {
            try
            {
                UserInfo user = new UserInfo()
                {
                    Email = email,
                    Password = password.GetHashCode().ToString(),
                    IsAccountVerified = false,
                };

                await mongoDBService.Connect();
                await mongoDBService.Insert(user);

                return user.ID;
            }
            catch (Exception ex)
            {
                await exceptionLogger.Log(new ApplicationError(ex.Message));
                throw new DatabaseException("Error occured when inserting a user in the database");
            }
        }
    }
}
