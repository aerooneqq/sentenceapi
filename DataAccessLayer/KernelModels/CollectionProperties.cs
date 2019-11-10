using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

using Newtonsoft.Json;

namespace DataAccessLayer.KernelModels
{
    public class CollectionProperties
    {
        [BsonId]
        public ObjectId _id { get; set; }
        [BsonElement("collectionName"), JsonProperty("collectionName")]
        public string CollectionName { get; set; }
        [BsonElement("lastID"), JsonProperty("lastID")]
        public long LastID { get; set; }
    }
}
