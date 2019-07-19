using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MongoDB.Bson.Serialization.Attributes;

namespace SentenceAPI.KernelModels
{
    public class UniqueEntity
    {
        [BsonId]
        public long ID { get; set; }
    }
}
