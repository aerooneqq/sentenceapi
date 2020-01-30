using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

using Newtonsoft.Json;


namespace Domain.Document.DocumentStatus
{
    public class DocumentAccess
    {
        [BsonElement("userID"), JsonProperty("userID")]
        public ObjectId UserID { get; set; }
        
        [BsonElement("role"), JsonProperty("role")]
        public DocumentUserRole Role { get; set; }
        
        [BsonElement("accessType"), JsonProperty("accessType")]
        public AccessType AccessType { get; set; }
        
        
        public DocumentAccess() {}

        public DocumentAccess(ObjectId userID, DocumentUserRole role, AccessType accessType)
        {
            UserID = userID;
            Role = role;
            AccessType = accessType;
        }
    }
}