﻿using MongoDB.Bson;

 using Newtonsoft.Json;
 
 using System.Collections.Generic;

 namespace Application.UserFeed.Models
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
