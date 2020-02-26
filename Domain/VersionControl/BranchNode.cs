using System;
using Domain.DocumentElements;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.VersionControl
{
    public class BranchNode
    {
        [BsonElement("branchNodeID")]
        public ObjectId BranchNodeID { get; set; }

        [BsonElement("creatorID")]
        public ObjectId CreatorID { get; set; }

        [BsonElement("updatedAt")] 
        public DateTime UpdatedAt { get; set; }

        [BsonElement("createdAt")] 
        public DateTime CreatedAt { get; set; }

        [BsonElement("comment")]
        public string Comment { get; set; }

        [BsonElement("title")]
        public string Title { get; set; }

        [BsonElement("documentElement")]
        public DocumentElement DocumentElement { get; set; }
    }
}
