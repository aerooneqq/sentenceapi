using MongoDB.Bson;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentenceAPI.Features.UserFeed.Models
{
    public class UsersFeedDto
    {
        [JsonProperty("usersFeed")]
        public List<object> UsersFeed { get; set; }

        [JsonProperty("userPhotoes")]
        public Dictionary<ObjectId, string> UserPhotoes { get; set; }

        #region Constructors
        public UsersFeedDto(List<object> usersFeed, Dictionary<ObjectId, string> userPhotoes)
        {
            UsersFeed = usersFeed;
            UserPhotoes = userPhotoes;
        }
        #endregion
    }
}
