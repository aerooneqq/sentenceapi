using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentenceAPI.Databases.MongoDB.Interfaces
{
    public interface IMongoDBServiceBuilder<DataType>
    {
        IMongoDBServiceBuilder<DataType> AddConfigurationFile();
        IMongoDBServiceBuilder<DataType> SetCollectionName();
        IMongoDBServiceBuilder<DataType> SetConnectionString();
        IMongoDBServiceBuilder<DataType> SetDatabaseName(string databaseName);

        IMongoDBService<DataType> Build();
    }
}
