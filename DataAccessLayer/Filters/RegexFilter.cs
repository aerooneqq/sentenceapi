using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using DataAccessLayer.Filters.Base;

using MongoDB.Bson;
using MongoDB.Driver;

namespace DataAccessLayer.Filters
{
    public class RegexFilter : FilterBase
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

        public override BsonDocument ToMongoBsonDocument()
        {
            throw new NotImplementedException();
        }

        public override FilterDefinition<DataType> ToMongoFilter<DataType>()
        {
            return Builders<DataType>.Filter.Regex(propertyName, regexPattern);
        }
    }
}
