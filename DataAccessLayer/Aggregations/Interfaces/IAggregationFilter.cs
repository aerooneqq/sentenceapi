using DataAccessLayer.Filters;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessLayer.Aggregations.Interfaces
{
    public interface IAggregationFilter
    {
        IAggregation Aggregation { get; }

        List<BsonDocument> ToBsonDocument();
    }
}
