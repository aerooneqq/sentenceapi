using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using Newtonsoft.Json;

namespace SentenceAPI.Features.UserFriends.Models
{
    public class Subscription
    {
        [JsonProperty("userID")]
        public ObjectId UserID { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; } 

        [JsonProperty("birthDate")]
        public DateTime BirthDate { get; set; }
    }
}
