using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Domain.DocumentStructureModels.ItemStatus
{
    public class ItemUserRole
    {
        [BsonElement("userID"), JsonProperty("userID")]
        public ObjectId UserID { get; set; }
        
        [BsonElement("access"), JsonProperty("access")]
        public ItemAccess Access { get; set; }
    }
}