using System;
using MongoDB.Bson;

namespace Domain.VersionControl
{
    public class BranchNode
    {
        public ObjectId CreatorID { get; set; } 
        public DateTime UpdatedAt { get; set; } 
        public DateTime CreatedAt { get; set; }
        public string Comment { get; set; }
        public string Title { get; set; }
        public byte[] State { get; set; }
    }
}
