using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace SentenceAPI.Databases.Filters
{
    public class InFilter<ValueType> : IFilter
    {
        private readonly string propertyName;
        private readonly IEnumerable<ValueType> values;

        public InFilter(string propertyName, IEnumerable<ValueType> values)
        {
            this.propertyName = propertyName;
            this.values = values;
        }

        public FilterDefinition<DataType> ToMongoFilter<DataType>()
        {
            return Builders<DataType>.Filter.In(propertyName, values);
        }
    }
}
