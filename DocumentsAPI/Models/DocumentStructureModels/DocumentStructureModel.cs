using System; 
using System.Collections.Generic;

using DataAccessLayer.KernelModels;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

using Newtonsoft.Json;

namespace DocumentsAPI.Models.DocumentStructureModels
{ 
    public class DocumentStructureModel : UniqueEntity
    { 
        [BsonElement("lastUpdatedAt"), JsonProperty("lastUpdatedAt")]
        public DateTime LastUpdatedAt { get; set; }

        [BsonElement("parentDocumentID"), JsonProperty("parentDocumentID")]
        public ObjectId ParentDocumentID { get; set; }

        [BsonElement("items"), JsonProperty("items")]
        public List<Item> Items { get; set; } 
    }
}