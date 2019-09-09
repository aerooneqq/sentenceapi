using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DataAccessLayer.Filters
{
    public class RegexFilter : IFilter
    {
        #region Fields
        private readonly string propertyName;
        private readonly string regexPattern;
        #endregion

        public RegexFilter(string propertyName, string regexPattern)
        {
            this.regexPattern = regexPattern;
            this.propertyName = propertyName;
        }

        public BsonDocument ToMongoBsonDocument()
        {
            throw new NotImplementedException();
        }

        public FilterDefinition<DataType> ToMongoFilter<DataType>()
        {
            return Builders<DataType>.Filter.Regex(propertyName, regexPattern);
        }
    }
}
