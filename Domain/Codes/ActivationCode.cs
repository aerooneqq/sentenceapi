using Domain.KernelModels;

using System;

using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

using Newtonsoft.Json;


namespace Domain.Codes
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
