using System;
using System.Collections.Generic;
using Domain.DocumentStructureModels;
using Domain.KernelModels;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Domain.Templates 
{
    public class TemplateItem
    {
        [BsonElement("name"), JsonProperty("name")]
        public string Name { get; set; }

        [BsonElement("itemType"), JsonProperty("itemType")]
        public ItemType ItemType { get; set; }

        [BsonElement("items"), JsonProperty("items")]
        public List<TemplateItem> Items { get; set; }

        [BsonElement("comment"), JsonProperty("comment")]
        public string Comment { get; set; }

        public TemplateItem() 
        {
            Items = new List<TemplateItem>();
        }

        public TemplateItem(string name, ItemType itemType) : this() 
        {
            Name = name;
            ItemType = itemType;
        }
    }
}