using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

using Newtonsoft.Json;

using System.Collections.Generic;

using Domain.JsonConverters;
using Domain.KernelModels;


namespace Domain.UserFriends
{
    public class UserFriends : UniqueEntity
    {
        [BsonElement("userID"), JsonProperty("userID"), JsonConverter(typeof(ObjectIDJsonConverter))]
        public ObjectId UserID { get; set; }

        [BsonElement("subscribersID"), JsonProperty("subscribersID")]
        public List<ObjectId> SubscribersID { get; set; }

        [BsonElement("subscriptionsID"), JsonProperty("subscriptionsID")]
        public List<ObjectId> SubscriptionsID { get; set; }
    }
}
