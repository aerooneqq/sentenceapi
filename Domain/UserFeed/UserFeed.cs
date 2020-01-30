using System;

using Newtonsoft.Json;

using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

using Domain.KernelModels;
using Domain.JsonConverters;


namespace Domain.UserFeed
{
    public class UserFeed : UniqueEntity
    {
        [BsonElement("userID"), JsonProperty("userID")]
        [JsonConverter(typeof(ObjectIDJsonConverter))]
        public ObjectId UserID { get; set; }

        [BsonElement("message"), JsonProperty("message")]
        public string Message { get; set; }

        [BsonElement("publicationDate"), JsonProperty("publicationDate")]
        public DateTime PublicationDate { get; set; }
    }
}
