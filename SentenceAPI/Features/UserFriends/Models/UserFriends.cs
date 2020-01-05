using DataAccessLayer.KernelModels;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentenceAPI.Features.UserFriends.Models
{
    public class UserFriends : UniqueEntity
    {
        [BsonElement("userID"), JsonProperty("userID")]
        public ObjectId UserID { get; set; }

        [BsonElement("subscribersID"), JsonProperty("subscribersID")]
        public List<ObjectId> SubscribersID { get; set; }

        [BsonElement("subscriptionsID"), JsonProperty("subscriptionsID")]
        public List<ObjectId> SubscriptionsID { get; set; }
    }
}
