using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MongoDB.Bson.Serialization.Attributes;

namespace DataAccessLayer.KernelModels
{
    public class UniqueEntity
    {
        [BsonId, BsonElement("_id")]
        public long ID { get; set; }
    }
}
