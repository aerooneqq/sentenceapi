using System;
using Domain.Date;
using Domain.DocumentElements;
using Domain.DocumentElements.Image;
using Domain.DocumentElements.NumberedList;
using Domain.DocumentElements.Paragraph;
using Domain.DocumentElements.Table;
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


        public static BranchNode GetEmptyNode(DocumentElementType type, IDateService dateService, ObjectId creatorID)
        {
            return new BranchNode()
            {
                BranchNodeID = ObjectId.GenerateNewId(),
                Comment = "Enter comment here",
                CreatedAt = dateService.Now,
                UpdatedAt = dateService.Now,
                CreatorID = creatorID,
                DocumentElement = type switch 
                {
                    DocumentElementType.Image => new Image(),
                    DocumentElementType.NumberedList => new NumberedList(),
                    DocumentElementType.Paragraph => new Paragraph(),
                    DocumentElementType.Table => new Table(), 
                    _ => null
                },
                Title = "New node",
            };
        } 
    }
}
