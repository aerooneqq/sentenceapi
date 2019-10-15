using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccessLayer.Filters.Base;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DataAccessLayer.Filters
{
    public class InFilter<ValueType> : FilterBase
    {
        private readonly string propertyName;
        private readonly IEnumerable<ValueType> values;

        public InFilter(string propertyName, IEnumerable<ValueType> values)
        {
            this.propertyName = propertyName;
            this.values = values;
        }

        public override BsonDocument ToMongoBsonDocument()
        {
            return new BsonDocument(propertyName, 
                new BsonDocument(new Dictionary<string, object>() { { "$in", values } }));
        }

        public override FilterDefinition<DataType> ToMongoFilter<DataType>()
        {
            return Builders<DataType>.Filter.In(propertyName, values);
        }
    }
}
