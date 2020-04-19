using System;
using System.Collections.Generic;
using System.Text;
using DataAccessLayer.Filters.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DataAccessLayer.Filters.Base
{
    class OrFilter : FilterBase
    {
        private readonly IFilter firstFilter;
        private readonly IFilter secondFilter;

        public OrFilter(IFilter firstFilter, IFilter secondFilter)
        {
            this.firstFilter = firstFilter;
            this.secondFilter = secondFilter;
        }

        public override BsonDocument ToMongoBsonDocument()
        {
            return new BsonDocument();
        }

        public override FilterDefinition<DataType> ToMongoFilter<DataType>()
        {
            return Builders<DataType>.Filter.Or(firstFilter.ToMongoFilter<DataType>(), 
                                                secondFilter.ToMongoFilter<DataType>());
        }
    }
}
