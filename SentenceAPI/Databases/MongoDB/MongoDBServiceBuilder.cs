﻿using Microsoft.Extensions.Configuration;
using SentenceAPI.Databases.CommonInterfaces;
using SentenceAPI.Databases.MongoDB.Interfaces;
using SentenceAPI.KernelModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SentenceAPI.Databases.MongoDB
{
    public class MongoDBServiceBuilder<DataType> : IMongoDBServiceBuilder<DataType>
        where DataType : UniqueEntity
    {
        #region Fields
        private readonly string collectionPostfix = "Collection";
        private readonly IMongoDBService<DataType> mongoDBService;
        #endregion

        #region Constructors
        public MongoDBServiceBuilder(IMongoDBService<DataType> mongoDBService)
        {
            this.mongoDBService = mongoDBService;
        }
        #endregion

        #region IMongoDBServiceBuilder<DataType> implementations
        public IMongoDBServiceBuilder<DataType> AddConfigurationFile(string filePath)
        {
            mongoDBService.Configuration = new ConfigurationBuilder().
                SetBasePath(Directory.GetCurrentDirectory()).
                AddJsonFile(filePath).Build();

            return this;
        }

        public IMongoDBServiceBuilder<DataType> SetCollectionName()
        {
            string collectionName = typeof(DataType).Name + collectionPostfix;
            mongoDBService.CollectionName = collectionName;
            mongoDBService.SupportCollectionName = "SupportCollection";
            mongoDBService.SupportDocumentName = collectionName + "Support";

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

        public IMongoDBServiceBuilder<DataType> SetUserName()
        {
            mongoDBService.UserName = mongoDBService.Configuration["userName"];
            return this;
        }

        public IMongoDBServiceBuilder<DataType> SetPassword()
        {
            mongoDBService.Password = mongoDBService.Configuration["password"];
            return this;
        }

        public IMongoDBServiceBuilder<DataType> SetAuthMechanism()
        {
            mongoDBService.AuthMechanism = mongoDBService.Configuration["authMechanism"];
            return this;
        }

        public IMongoDBService<DataType> Build()
        {
            return mongoDBService;
        }
        #endregion
    }
}
