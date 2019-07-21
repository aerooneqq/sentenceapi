using SentenceAPI.Databases.MongoDB.Interfaces;
using SentenceAPI.KernelModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentenceAPI.Databases.MongoDB.Factories
{
    public class MongoDBServiceFactory : IMongoDBServiceFactory
    {
        public IMongoDBServiceBuilder<DataType> GetBuilder<DataType>(
            IMongoDBService<DataType> mongoDBService) where DataType : UniqueEntity
        {
            return new MongoDBServiceBuilder<DataType>(mongoDBService);
        }

        public IMongoDBService<DataType> GetService<DataType>()
            where DataType : UniqueEntity
        {
            return new MongoDBService<DataType>();
        }
    }
}
