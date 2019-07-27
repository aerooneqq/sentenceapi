using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using SentenceAPI.KernelModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentenceAPI.Features.UserFriends.Models
{
    public class UserFriends : UniqueEntity
    {
        [BsonElement("userID"), JsonProperty("userID")]
        public long UserID { get; set; }

        [BsonElement("subscribersID"), JsonProperty("subscribersID")]
        public List<long> SubscribersID { get; set; }

        [BsonElement("subscriptionsID"), JsonProperty("subscriptionsID")]
        public List<long> SubscriptionsID { get; set; }
    }
}
