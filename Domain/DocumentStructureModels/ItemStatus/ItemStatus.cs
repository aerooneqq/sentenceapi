using System.Collections.Generic;

using MongoDB.Bson.Serialization.Attributes;

using Newtonsoft.Json;


namespace Domain.DocumentStructureModels.ItemStatus
{
    public class ItemStatus
    {
        [BsonElement("itemType"), JsonProperty("itemType")]
        public ItemType ItemType { get; set; }

        [BsonElement("accesses"), JsonProperty("accesses")]
        public List<ItemUserRole> Accesses { get; set; }

        [BsonElement("isExpanded"), JsonProperty("isExpanded")]
        public bool IsExpanded { get; set; }
    }
}