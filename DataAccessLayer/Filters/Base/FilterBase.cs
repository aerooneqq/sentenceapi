using System;
using System.Collections.Generic;
using System.Text;
using DataAccessLayer.Filters.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DataAccessLayer.Filters.Base
{
    public abstract class FilterBase : IFilter
    {
        public abstract FilterDefinition<DataType> ToMongoFilter<DataType>();
        public abstract BsonDocument ToMongoBsonDocument();

        public static FilterBase operator | (FilterBase firstFilter, FilterBase secondFilter)
        {
            return new OrFilter(firstFilter, secondFilter);
        }

        public static FilterBase operator & (FilterBase firstFilter, FilterBase secondFilter)
        {
            return new AndFilter(firstFilter, secondFilter);
        }
    }
}
