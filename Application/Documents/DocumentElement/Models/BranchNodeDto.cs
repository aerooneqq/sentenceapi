using System;
using Domain.DocumentElements;
using Domain.VersionControl;
using MongoDB.Bson;
using Newtonsoft.Json;

namespace Application.Documents.DocumentElement.Models
{
    public class BranchNodeDto
    {
        [JsonProperty("branchNodeID")]
        public ObjectId BranchNodeID { get; set; }

        [JsonProperty("creatorID")]
        public ObjectId CreatorID { get; set; }

        [JsonProperty("updatedAt")] 
        public DateTime UpdatedAt { get; set; }

        [JsonProperty("createdAt")] 
        public DateTime CreatedAt { get; set; }

        [JsonProperty("comment")]
        public string Comment { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("documentElement")]
        public Domain.DocumentElements.DocumentElement DocumentElement { get; set; }

        
        public BranchNodeDto(BranchNode node) 
        {
            BranchNodeID = node.BranchNodeID;
            CreatorID = node.CreatorID;
            UpdatedAt = node.UpdatedAt;
            CreatedAt = node.CreatedAt;
            Comment = node.Comment;
            Title = node.Title;
            DocumentElement = node.DocumentElement;
        }
    }
}