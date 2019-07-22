using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MongoDB.Bson.Serialization.Attributes;

using Newtonsoft.Json;

namespace SentenceAPI.Features.UserActivity.Models
{
    public class SingleUserActivity
    {
        [BsonElement("activityDate"), JsonProperty("activityDate")]
        public DateTime ActivityDate { get; set; }

        [BsonElement("activity"), JsonProperty("activity")]
        public string Activity { get; set; }
    }
}
