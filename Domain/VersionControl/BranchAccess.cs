using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.VersionControl
{
    public class BranchAccess
    {
        [BsonElement("userID")]
        public ObjectId UserID { get; set; }

        [BsonElement("accessType")]
        public BranchAccessType AccessType { get; set; }
    }
}