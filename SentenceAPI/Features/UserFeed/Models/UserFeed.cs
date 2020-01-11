using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Newtonsoft.Json;

using MongoDB.Bson.Serialization.Attributes;

using DataAccessLayer.KernelModels;
using MongoDB.Bson;

using SharedLibrary.JsonConverters;


namespace SentenceAPI.Features.UserFeed.Models
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
