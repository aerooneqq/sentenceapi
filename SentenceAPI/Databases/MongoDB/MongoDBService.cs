using SentenceAPI.Databases.MongoDB.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

using MongoDB.Bson;
using MongoDB.Driver;

using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols;

namespace SentenceAPI.Databases.MongoDB
{
    public class MongoDBService<DataType> : IMongoDBService<DataType>
    {
        #region Fields
        private MongoClient mongoClient;
        private IMongoDatabase database;
        private IMongoCollection<DataType> mongoCollection;
        #endregion

        #region Properties
        public string DatabaseName { get; set; }
        public string CollectionName { get; set; }
        public string ConnectionString { get; set; }
        public IConfiguration Configuration { get; set; }
        #endregion

        #region Constructors
        public MongoDBService() { }
        #endregion

        #region IMongoDBService<DataType> implementation
        public async Task Connect()
        {
            await Task.Run(() =>
            {
                mongoClient = new MongoClient(ConnectionString);
                database = mongoClient.GetDatabase(DatabaseName);
            });
        }

        public Task Delete(int id)
        {
            throw new NotImplementedException();    
        }

        public Task Disconnect()
        {
            throw new NotImplementedException();
        }

        public Task<DataType> Get(int id)
        {
            throw new NotImplementedException();
        }

        public Task<DataType> Get(Dictionary<string, object> properties)
        {
            mongoCollection = database.GetCollection<DataType>();
        }

        public Task Insert(DataType entity)
        {
            throw new NotImplementedException();
        }

        public Task Update(DataType entity)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
