using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace SentenceAPI.Databases.Filters
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

        public FilterDefinition<DataType> ToMongoFilter<DataType>()
        {
            return Builders<DataType>.Filter.Eq(propertyName, value);
        }
    }
}
