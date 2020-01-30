using Newtonsoft.Json;

using MongoDB.Bson.Serialization.Attributes;


namespace Domain.Users
{
    /// <summary>
    /// This class represents one of the Career stage of a employee
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
        public int StartYear { get; set; }

        [BsonElement("finishYear"), JsonProperty("finishYear")]
        public int FinishYear { get; set; }
    }
}
