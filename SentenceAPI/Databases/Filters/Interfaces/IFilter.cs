using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentenceAPI.Databases.Filters
{
    /// <summary>
    /// IFilter is used to select suitable data from the database. 
    /// </summary>
    public interface IFilter
    {
        FilterDefinition<DataType> ToMongoFilter<DataType>();
    }
}
