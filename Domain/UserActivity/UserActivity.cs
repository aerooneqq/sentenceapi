using System;
using System.Collections.Generic;

using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

using Newtonsoft.Json;

using Domain.KernelModels;
using Domain.JsonConverters;


namespace Domain.UserActivity
{
    public class UserActivity : UniqueEntity
    {
        [BsonElement("userID"), JsonProperty("userID")]
        [JsonConverter(typeof(ObjectIDJsonConverter))]
        public ObjectId UserID { get; set; }

        [BsonElement("lastActivityDate"), JsonProperty("lastActivityDate")]
        public DateTime LastActivityDate { get; set; }

        [BsonElement("activities"), JsonProperty("activities")]
        public IEnumerable<SingleUserActivity> Activities { get; set; }

        [BsonElement("lastOnline"), JsonProperty("lastOnline")]
        public DateTime LastOnline { get; set; }

        [BsonElement("isOnline"), JsonProperty("isOnline")]
        public bool IsOnline { get; set; }
    }
}
