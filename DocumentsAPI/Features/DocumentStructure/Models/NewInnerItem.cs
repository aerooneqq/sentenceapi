using DocumentsAPI.Models.DocumentStructureModels;

using Newtonsoft.Json;


namespace DocumentsAPI.Features.DocumentStructure.Models
{
    public class NewInnerItem
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("itemType")]
        public ItemType ItemType { get; set; }

        [JsonProperty("position")]
        public int Position { get; set; }


        public NewInnerItem() { }
        public NewInnerItem(string name, ItemType itemType)
        {
            Name = name;
            ItemType = itemType;
        }
    }
}