using DataAccessLayer.Filters;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessLayer.Aggregations.Interfaces
{
    /// <summary>
    /// This interface is similar to IFilter interface. IAggregationFilter represents the mechanism which
    /// will create the instructions on Aggregation for every database.
    /// </summary>
    public interface IAggregationFilter
    {
        IAggregation Aggregation { get; }

        List<BsonDocument> ToBsonDocument();
    }
}
