using Domain.JsonConverters;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

using Newtonsoft.Json;


namespace Domain.Workplace.DocumentsDeskState
{
    public class DocumentTopBarInfo
    {
        [BsonElement("documentID"), JsonProperty("documentID")]
        [JsonConverter(typeof(ObjectIDJsonConverter))]
        public ObjectId DocumentID { get; set; }

        [BsonElement("documentName"), JsonProperty("documentName")]
        public string DocumentName { get; set; }
    }
}
