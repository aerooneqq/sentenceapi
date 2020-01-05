using DataAccessLayer.KernelModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MongoDB.Bson.Serialization.Attributes;

using Newtonsoft.Json;
using MongoDB.Bson;

namespace SentenceAPI.Features.Codes.Models
{
    public class ActivationCode : UniqueEntity
    {
        [BsonElement("code"), JsonProperty("code")]
        public string Code { get; set; }

        [BsonElement("used"), JsonProperty("used")]
        public bool Used { get; set; } 

        [BsonElement("creationDate"), JsonProperty("creationDate")]
        public DateTime? CreationDate { get; set; }

        [BsonElement("usageDate"), JsonProperty("usageDate")]
        public DateTime? UsageDate { get; set; }

        [BsonElement("userID"), JsonProperty("userID")]
        public ObjectId UserID { get; set; }
    }
}
