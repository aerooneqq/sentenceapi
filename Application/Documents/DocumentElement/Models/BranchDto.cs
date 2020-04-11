using System;
using System.Collections.Generic;
using System.Linq;
using Domain.DocumentElements;
using Domain.VersionControl;
using MongoDB.Bson;
using Newtonsoft.Json;

namespace Application.Documents.DocumentElement.Models
{
    public class BranchDto
    {
        [JsonProperty("branchID")]
        public ObjectId BranchID { get; set; }

        [JsonProperty("author")]
        public ObjectId Author { get; set; }

        [JsonProperty("branchName")]
        public string BranchName { get; set; }

        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("updatedAt")]
        public DateTime UpdatedAt { get; set; } 
        
        [JsonProperty("currentNodeID")]
        public ObjectId CurrentBranchNodeID { get; set; }

        [JsonProperty("branchNodes")]
        public List<BranchNodeDto> BranchNodes { get; set; }


        public BranchDto(Branch branch) 
        {
            BranchID = branch.BranchID;
            Author = branch.Author;
            BranchName = branch.BranchName;
            CreatedAt = branch.CreatedAt;
            UpdatedAt = branch.UpdatedAt;
            CurrentBranchNodeID = branch.CurrentBranchNodeID;
            BranchNodes = branch.BranchNodes.Select(node => new BranchNodeDto(node)).ToList();
        }
    }
}