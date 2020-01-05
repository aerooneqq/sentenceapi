using System;
using System.Collections.Generic;

using DataAccessLayer.KernelModels;

using MongoDB.Bson.Serialization.Attributes;

using Newtonsoft.Json;

namespace DocumentsAPI.Models.DocumentStructureModels 
{ 
    /// <summary>
    /// This is the class which represents the document structure tree item 
    /// </summary>
    public class Item : UniqueEntity 
    { 
        [BsonElement("documentID"), JsonProperty("documentID")]
        public long DocumentID { get; set;}

        [BsonElement("name"), JsonProperty("name")]
        public string Name { get; set; }

        [BsonElement("itemType"), JsonProperty("itemType")]
        public ItemType ItemType { get; set; }

        [BsonElement("createdAt"), JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }

        [BsonElement("updatedAt"), JsonProperty("updatedAt")]
        public DateTime UpdatedAt { get; set; }

        [BsonElement("items"), JsonProperty("items")]
        public List<Item> Items { get; set; }
    }
}