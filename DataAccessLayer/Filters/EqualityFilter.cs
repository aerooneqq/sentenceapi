using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DataAccessLayer.Filters
{
    public class EqualityFilter<EntityType> : IFilter
    {
        #region Fields
        private readonly string propertyName;
        private readonly EntityType value;
        #endregion

        public EqualityFilter(string propertyName, EntityType value)
        {
            this.propertyName = propertyName;
            this.value = value;
        }

        public BsonDocument ToMongoBsonDocument()
        {
            return new BsonDocument(new Dictionary<string, object>() { { propertyName, value } });
        }

        public FilterDefinition<DataType> ToMongoFilter<DataType>()
        {
            return Builders<DataType>.Filter.Eq(propertyName, value);
        }
    }
}
