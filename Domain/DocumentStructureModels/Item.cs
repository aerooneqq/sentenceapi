using System;
using System.Collections.Generic;

using Domain.KernelModels;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

using Newtonsoft.Json;


namespace Domain.DocumentStructureModels 
{ 
    /// <summary>
    /// This is the class which represents the document structure tree item 
    /// </summary>
    public class Item : UniqueEntity 
    { 
        [BsonElement("documentID"), JsonProperty("documentID")]
        public ObjectId DocumentID { get; set;}

        [BsonElement("name"), JsonProperty("name")]
        public string Name { get; set; }

        [BsonElement("createdAt"), JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }

        [BsonElement("updatedAt"), JsonProperty("updatedAt")]
        public DateTime UpdatedAt { get; set; }

        [BsonElement("items"), JsonProperty("items")]
        public List<Item> Items { get; set; }

        [BsonElement("itemStatus"), JsonProperty("itemStatus")]
        public ItemType ItemType { get; set; }

        [BsonElement("itemStatus"), JsonProperty("itemStatus")]
        public ItemStatus.ItemStatus ItemStatus { get; set; }
    }
}