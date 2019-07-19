using Microsoft.Extensions.Configuration;
using SentenceAPI.Databases.MongoDB.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SentenceAPI.Databases.MongoDB
{
    public class MongoDBServiceBuilder<DataType> : IMongoDBServiceBuilder<DataType>
    {
        #region Fields
        private string collectionPostfix = "Collection";
        private IMongoDBService<DataType> mongoDBService;
        #endregion

        #region Constructors
        public MongoDBServiceBuilder(IMongoDBService<DataType> mongoDBService)
        {
            this.mongoDBService = mongoDBService;
        }
        #endregion

        #region IMongoDBServiceBuilder<DataType> implementations
        public IMongoDBServiceBuilder<DataType> AddConfigurationFile()
        {
            mongoDBService.Configuration = new ConfigurationBuilder().
                SetBasePath(Directory.GetCurrentDirectory()).
                AddJsonFile("database_config.json").Build();

            return this;
        }

        public IMongoDBServiceBuilder<DataType> SetCollectionName()
        {
            string collectionName = typeof(DataType).Name + collectionPostfix;
            mongoDBService.CollectionName = collectionName;

            return this;
        }

        public IMongoDBServiceBuilder<DataType> SetConnectionString()
        {
            string connectionString = mongoDBService.Configuration["mongoDBConStr"];
            mongoDBService.ConnectionString = connectionString;

            return this; 
        }

        public IMongoDBServiceBuilder<DataType> SetDatabaseName(string databaseName)
        {
            mongoDBService.DatabaseName = databaseName;

            return this;
        }

        public IMongoDBService<DataType> Build()
        {
            return mongoDBService;
        }
        #endregion
    }
}
