using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessLayer.Filters.Interfaces
{
    /// <summary>
    /// IFilter is used to select suitable data from the database. 
    /// </summary>
    public interface IFilter
    {
        FilterDefinition<DataType> ToMongoFilter<DataType>();
        BsonDocument ToMongoBsonDocument();
    }
}
