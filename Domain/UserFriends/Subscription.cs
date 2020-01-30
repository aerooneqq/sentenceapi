using System;

using MongoDB.Bson;

using Newtonsoft.Json;

using Domain.JsonConverters;


namespace Domain.UserFriends
{
    public class Subscription
    {
        [JsonProperty("userID"), JsonConverter(typeof(ObjectIDJsonConverter))]
        public ObjectId UserID { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; } 

        [JsonProperty("birthDate")]
        public DateTime BirthDate { get; set; }
    }
}
