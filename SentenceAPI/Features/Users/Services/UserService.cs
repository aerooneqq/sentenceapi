using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using System;

using MongoDB.Bson.Serialization.Attributes;

using SentenceAPI.Databases.MongoDB.Interfaces;
using SentenceAPI.Features.Users.Interfaces;
using SentenceAPI.Features.Users.Models;

namespace SentenceAPI.Features.Users.Services
{
    public class UserService : IUserService<UserInfo>
    {
        #region Fields
        private readonly IMongoDBService<UserInfo> mongoDBService;
        #endregion

        #region Builders
        private readonly IMongoDBServiceBuilder<UserInfo> mongoDBServiceBuilder;
        #endregion

        #region Factories
        private readonly FactoriesManager.FactoriesManager factoriesManager =
            FactoriesManager.FactoriesManager.Instance;
        private readonly IMongoDBServiceFactory mongoDBServiceFactory;
        #endregion

        #region Constructors
        public UserService()
        {
            mongoDBServiceFactory = factoriesManager[typeof(IMongoDBServiceFactory)].Factory
                as IMongoDBServiceFactory;

            mongoDBService = mongoDBServiceFactory.GetService<UserInfo>();
            mongoDBServiceBuilder = mongoDBServiceFactory.GetBuilder(mongoDBService);

            mongoDBService = mongoDBServiceBuilder.AddConfigurationFile("database_config.json")
                .SetConnectionString()
                .SetDatabaseName("SentenceDatabase")
                .SetCollectionName()
                .Build();
        }
        #endregion

        public void Delete(long id)
        {
            throw new NotImplementedException();
        }

        public async Task<UserInfo> Get(string email, string password)
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

        private string GetPropertyBsonElement(string propertyName)
        {
            return typeof(UserInfo).GetProperty(propertyName).GetCustomAttribute<BsonElementAttribute>().
                ElementName;
        }

        public void Insert(UserInfo user)
        {
            throw new NotImplementedException();
        }

        public void Update(UserInfo user)
        {
            throw new NotImplementedException();
        }
    }
}
