using MongoDB.Bson;

namespace Domain.VersionControl
{
    public class BranchAccess
    {
        public ObjectId UserID { get; set; }
        public BranchAccessType AccessType { get; set; }
    }
}