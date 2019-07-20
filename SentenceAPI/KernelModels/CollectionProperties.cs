using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

using Newtonsoft.Json;

namespace SentenceAPI.KernelModels
{
    public class CollectionProperties
    {
        public object _id;
        [BsonElement("collectionName"), JsonProperty("collectionName")]
        public string CollectionName { get; set; }
        [BsonElement("lastID"), JsonProperty("lastID")]
        public long LastID { get; set; }
    }
}
