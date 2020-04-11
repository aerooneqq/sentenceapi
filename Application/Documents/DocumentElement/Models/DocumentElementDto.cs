using System;
using System.Collections.Generic;
using System.Linq;
using Domain.DocumentElements;
using Domain.VersionControl;

using MongoDB.Bson;

using Newtonsoft.Json;


namespace Application.Documents.DocumentElement.Models
{
    public class DocumentElementDto
    {
        [JsonProperty("elementID")]
        public ObjectId ElementID { get; set; }
        
        [JsonProperty("parentDocumentID")]
        public ObjectId ParentDocumentID { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("updatedAt")]
        public DateTime UpdatedAt { get; set; }

        [JsonProperty("branches")]
        public List<BranchDto> Branches { get; private set; }

        [JsonProperty("currentBranchID")]
        public ObjectId CurrentBranchID { get; set; }

        [JsonProperty("currentBranchNodeID")]
        public ObjectId CurrentBranchNodeID { get; set; }


        public DocumentElementDto(DocumentElementWrapper wrapper)
        {
            ElementID = wrapper.ID;
            ParentDocumentID = wrapper.ParentDocumentID;
            Type = wrapper.Type.ToString();
            CreatedAt = wrapper.CreatedAt;
            UpdatedAt = wrapper.UpdatedAt;
            Branches = new List<BranchDto >();
            CurrentBranchID = wrapper.CurrentBranchID;
            CurrentBranchNodeID = wrapper.CurrentBranchNodeID;
        }


        public void SetBranches(IEnumerable<Branch> allBranches, ObjectId userID) 
        {
            if (Branches is null)
                Branches = new List<BranchDto>();

            Branches = allBranches.Where(branch => 
            {
                BranchAccess access = branch.Accesses.FirstOrDefault(a => a.UserID == userID);
                return access is {} && access.AccessType != BranchAccessType.NoAccess;
            }).Select(branch => new BranchDto(branch)).ToList();
        }
    }
}