using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace SentenceAPI.Features.UserFriends.Models
{
    public class Subscription
    {
        [JsonProperty("userID")]
        public long UserID { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; } 

        [JsonProperty("birthDate")]
        public DateTime BirthDate { get; set; }
    }
}
