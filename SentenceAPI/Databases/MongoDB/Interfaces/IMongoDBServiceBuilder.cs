using SentenceAPI.Databases.CommonInterfaces;
using SentenceAPI.KernelModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentenceAPI.Databases.MongoDB.Interfaces
{
    public interface IMongoDBServiceBuilder<DataType>
        where DataType : UniqueEntity
    {
        IMongoDBServiceBuilder<DataType> AddConfigurationFile(string filePath);
        IMongoDBServiceBuilder<DataType> SetCollectionName();
        IMongoDBServiceBuilder<DataType> SetConnectionString();
        IMongoDBServiceBuilder<DataType> SetDatabaseName(string databaseName);
        IMongoDBServiceBuilder<DataType> SetUserName();
        IMongoDBServiceBuilder<DataType> SetPassword();
        IMongoDBServiceBuilder<DataType> SetAuthMechanism();

        IMongoDBService<DataType> Build();
    }
}
