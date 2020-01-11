using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SharedLibrary.JsonConverters;


namespace SentenceAPI.Features.Workplace.DocumentsDeskState.Models
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
