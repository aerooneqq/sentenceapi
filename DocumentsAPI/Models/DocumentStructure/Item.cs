using System;
using DataAccessLayer.Models;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace DocumentsAPI.Models.DocumentStructure 
{ 
    /// <summary>
    /// This is the class which represents the document structure tree item 
    /// </summary>
    public class Item : UniqueEntity 
    { 
        [BsonElement("documentID"), JsonProperty("documentID")]
        public long DocumentID { get; set;}

        [BsonElement("parentItemID"), JsonProperty("parentItemID")]
        public long ParentItemID { get; set;}

        [BsonElement("name"), JsonProperty("name")]
        public string Name { get; set; }

        [BsonElement("itemType"), JsonProperty("itemType")]
        public ItemType ItemType { get; set; }

        [BsonElement("createdAt"), JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }

        [BsonElement("updatedAt"), JsonProperty("updatedAt")]
        public DateTime UpdatedAt { get; set; }
    }
}