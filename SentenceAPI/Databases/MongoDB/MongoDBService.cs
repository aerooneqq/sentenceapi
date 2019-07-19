using SentenceAPI.Databases.MongoDB.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentenceAPI.Databases.MongoDB
{
    public class MongoDBService<DataType> : IMongoDBService<DataType>
    {
        private string collectionName;
        private string connectionString;

        public MongoDBService()
        {

        }

        public Task Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Task<DataType> Get(int id)
        {
            throw new NotImplementedException();
        }

        public Task<DataType> Get(Dictionary<string, object> properties)
        {
            throw new NotImplementedException();
        }

        public Task Insert(DataType entity)
        {
            throw new NotImplementedException();
        }

        public Task Update(DataType entity)
        {
            throw new NotImplementedException();
        }
    }
}
