using System;
using System.Collections.Generic;

using Domain.KernelModels;
using Domain.Templates;
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
        [BsonElement("name"), JsonProperty("name")]
        public string Name { get; set; }

        [BsonElement("createdAt"), JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }

        [BsonElement("updatedAt"), JsonProperty("updatedAt")]
        public DateTime UpdatedAt { get; set; }

        [BsonElement("items"), JsonProperty("items")]
        public List<Item> Items { get; set; }

        [BsonElement("elementOrders"), JsonProperty("elementOrder")]
        public List<ObjectId> ElementsIds { get; set; }

        [BsonElement("itemStatus"), JsonProperty("itemStatus")]
        public ItemStatus.ItemStatus ItemStatus { get; set; }

        [BsonElement("comment"), JsonProperty("comment")]
        public string Comment { get; set; }

        
        public Item()
        {
            ElementsIds = new List<ObjectId>();
        }

        public Item(TemplateItem templateItem)
        {
            Name = templateItem.Name;
            Items = new List<Item>();
            ElementsIds = new List<ObjectId>();
            Comment = templateItem.Comment;
            ItemStatus = new ItemStatus.ItemStatus()
            {
                ItemType = templateItem.ItemType,
            };
        }
    }
}