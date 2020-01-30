using System.Collections.Generic;

using Domain.Models.Document;

using MongoDB.Bson.Serialization.Attributes;

using Newtonsoft.Json;


namespace Domain.Document.DocumentStatus
{
    public class DocumentStatus
    {
        [BsonElement("documentType"), JsonProperty("documentType")]
        public DocumentType DocumentType { get; set; }
        
        [BsonElement("accesses"), JsonProperty("accesses")]
        public List<DocumentAccess> Accesses { get; set; }
    }
}