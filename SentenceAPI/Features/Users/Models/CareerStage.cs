using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Newtonsoft.Json;

using MongoDB.Bson.Serialization.Attributes;

namespace SentenceAPI.Features.Users.Models
{
    /// <summary>
    /// This class represnets one of the Career stage of a employee
    /// </summary>
    public class CareerStage
    {
        [BsonElement("company"), JsonProperty("company")]
        public string Company { get; set; }

        [BsonElement("job"), JsonProperty("job")]
        public string Job { get; set; }

        [BsonElement("description"), JsonProperty("description")]
        public string Description { get; set; }

        [BsonElement("startYear"), JsonProperty("startYear")]
        public DateTime StartYear { get; set; }

        [BsonElement("finishYear"), JsonProperty("finishYear")]
        public DateTime FinishYear { get; set; }
    }
}
