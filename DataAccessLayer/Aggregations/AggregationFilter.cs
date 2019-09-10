using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using DataAccessLayer.Attributes;
using DataAccessLayer.Filters;
using DataAccessLayer.Aggregations.Interfaces;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DataAccessLayer.Aggregations
{
    /// <summary>
    /// Transform the aggregation query to the request to a perticular database.
    /// </summary>
    public class AggregationFilter : IAggregationFilter
    {
        public IAggregation Aggregation { get; }

        public AggregationFilter(IAggregation aggregation)
        {
            Aggregation = aggregation;
        }

        public List<BsonDocument> ToBsonDocument()
        {
            List<BsonDocument> pipelineDocs = new List<BsonDocument>();

            foreach (string colName in Aggregation.ExtraCollections.Keys)
            {
                pipelineDocs.Add(new BsonDocument("$lookup", new BsonDocument
                {
                    { "from", colName },
                    { "localField", Aggregation.LocalField },
                    { "foreignField", Aggregation.ExtraCollections[colName].foreignKey },
                    { "as", colName + "Result" }
                }));

                pipelineDocs.Add(new BsonDocument("$unwind", "$" + colName + "Result"));
            }

            pipelineDocs.Add(new BsonDocument("$project", CreateProjectionDocument()));

            return pipelineDocs;
        }

        private BsonDocument CreateProjectionDocument()
        {
            var projectionDocument = new BsonDocument();

            foreach (PropertyInfo property in Aggregation.MainType.GetTypeInfo().GetProperties())
            {
                if (property.GetCustomAttribute<SecretAttribute>() == null)
                {
                    string bsonPropertyName = property.GetCustomAttribute<BsonElementAttribute>()?.ElementName;
                    projectionDocument.Add(bsonPropertyName, 1);
                }
            }

            foreach (string extraColName in Aggregation.ExtraCollections.Keys)
            {
                foreach (string requestedProperty in Aggregation.ExtraCollections[extraColName].requestedProperties)
                {
                    projectionDocument.Add(requestedProperty, $"${extraColName}Result.{requestedProperty}");
                }
            }

            return projectionDocument;
        }
    }
}
