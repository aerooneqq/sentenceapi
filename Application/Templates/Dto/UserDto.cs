using System;
using System.Collections.Generic;
using Domain.JsonConverters;
using Domain.Templates;
using MongoDB.Bson;
using Newtonsoft.Json;

namespace Application.Templates
{
    public readonly struct UserDto
    {
        [JsonProperty("userID")]
        public ObjectId UserID { get; }

        [JsonProperty("username")]
        public string Username { get; }

        public UserDto(ObjectId userID, string username)
        {
            UserID = userID;
            Username = username;
        }
    }
}