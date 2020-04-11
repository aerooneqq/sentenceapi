using System;
using System.Collections.Generic;
using Domain.Date;
using Domain.DocumentElements;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

using Newtonsoft.Json;

namespace Domain.VersionControl
{
    public class Branch
    {
        [BsonElement("branchID"), JsonProperty("branchID")]
        public ObjectId BranchID { get; set; }

        [BsonElement("author"), JsonProperty("author")]
        public ObjectId Author { get; set; }

        [BsonElement("branchName"), JsonProperty("branchName")]
        public string BranchName { get; set; }

        [BsonElement("createdAt"), JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }

        [BsonElement("updatedAt"), JsonProperty("updatedAt")]
        public DateTime UpdatedAt { get; set; } 
        
        [BsonElement("currentNodeID"), JsonProperty("currentNodeID")]
        public ObjectId CurrentBranchNodeID { get; set; }

        [BsonElement("accesses"), JsonProperty("accesses")]
        public List<BranchAccess> Accesses { get; set; }

        [BsonElement("branchNodes"), JsonProperty("branchNodes")]
        public List<BranchNode> BranchNodes { get; set; }



        public static Branch GetNewBranch(string branchName, IDateService dateService, DocumentElementType elementType,
                                          List<BranchAccess> accesses, ObjectId creatorID)
        {
            BranchNode firstNode = BranchNode.GetEmptyNode(elementType, dateService, creatorID);

            return new Branch()
            {
                Accesses = accesses,
                Author = creatorID,
                BranchID = ObjectId.GenerateNewId(),
                BranchName = branchName,
                BranchNodes = new List<BranchNode>() {firstNode},
                CreatedAt = dateService.Now, 
                CurrentBranchNodeID = firstNode.BranchNodeID,
                UpdatedAt = dateService.Now,
            };
        }
    }
}