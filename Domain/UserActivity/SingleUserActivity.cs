using System;

using MongoDB.Bson.Serialization.Attributes;

using Newtonsoft.Json;


namespace Domain.UserActivity
{
    public class SingleUserActivity
    {
        [BsonElement("activityDate"), JsonProperty("activityDate")]
        public DateTime ActivityDate { get; set; }

        [BsonElement("activity"), JsonProperty("activity")]
        public string Activity { get; set; }
    }
}
