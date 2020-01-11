using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MongoDB.Bson.Serialization.Attributes;

using Newtonsoft.Json;

using DataAccessLayer.KernelModels;
using MongoDB.Bson;

using SharedLibrary.JsonConverters;


namespace SentenceAPI.Features.UserActivity.Models
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
