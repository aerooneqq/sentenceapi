using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

using Newtonsoft.Json;

using System.Collections.Generic;

using Domain.JsonConverters;
using Domain.KernelModels;


namespace Domain.Workplace.DocumentsDeskState
{
    /// <summary>
    /// Represents the state of the workplace top header, object of this class show which documents are
    /// opened in the users' workplace
    /// </summary>
    public class DocumentDeskState : UniqueEntity
    {
        [BsonElement("userID"), JsonProperty("userID"), JsonConverter(typeof(ObjectIDJsonConverter))]
        public ObjectId UserID { get; set; }
        
        [BsonElement("documentTopBarInfos"), JsonProperty("documentTopBarInfos")]
        public IEnumerable<DocumentTopBarInfo> DocumentTopBarInfos { get; set; } 

        [BsonElement("openedDocumentID"), JsonProperty("openedDocumentID")]
        [JsonConverter(typeof(ObjectIDJsonConverter))]
        public ObjectId OpenedDocumentID { get; set; }
    }
}
