using MongoDB.Bson;

using Newtonsoft.Json;


namespace Application.Documents.DocumentStructure.Models
{
    public class ItemUpdateDto
    {
        [JsonProperty("parentDocumentStructureID")]
        public ObjectId ParentDocumentStructureID { get; set; }

        [JsonProperty("itemID")]
        public ObjectId ItemID { get; set; }

        [JsonProperty("newName")]
        public string NewName { get; set; }

        [JsonProperty("newInnerItem")]
        public NewInnerItem NewInnerItem { get; set; }
    }
}