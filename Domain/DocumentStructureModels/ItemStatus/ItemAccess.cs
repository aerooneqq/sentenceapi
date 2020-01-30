using MongoDB.Bson;

using MongoDB.Bson.Serialization.Attributes;

using Newtonsoft.Json;


namespace Domain.DocumentStructureModels.ItemStatus
{
    public class ItemAccess
    {
        [BsonElement("userID"), JsonProperty("userID")]
        public ObjectId UserID { get; set; }
        
        [BsonElement("accessType"), JsonProperty("accessType")]
        public AccessType AccessType { get; set; }
    }
}