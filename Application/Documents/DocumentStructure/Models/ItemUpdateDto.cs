using Domain.JsonConverters;

using MongoDB.Bson;

using Newtonsoft.Json;


namespace Application.Documents.DocumentStructure.Models
{
    public class ItemUpdateDto
    {
        [JsonProperty("parentDocumentStructureID"), JsonConverter(typeof(ObjectIDJsonConverter))]
        public ObjectId ParentDocumentStructureID { get; set; }

        [JsonProperty("itemID"), JsonConverter(typeof(ObjectIDJsonConverter))]
        public ObjectId ItemID { get; set; }

        [JsonProperty("newName")]
        public string NewName { get; set; }

        [JsonProperty("newInnerItem")]
        public NewInnerItem NewInnerItem { get; set; }
    }
}