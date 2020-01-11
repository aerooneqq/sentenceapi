using DataAccessLayer.KernelModels;
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
