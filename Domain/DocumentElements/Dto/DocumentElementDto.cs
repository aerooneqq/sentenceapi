using System;
using System.Collections.Generic;
using System.Linq;
using Domain.VersionControl;

using MongoDB.Bson;

using Newtonsoft.Json;


namespace Domain.DocumentElements.Dto
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

        [JsonProperty("document")]
        public List<Branch> Branches { get; private set; }


        /// <summary>
        /// Initializes all 
        /// </summary>
        public DocumentElementDto(DocumentElementWrapper wrapper)
        {
            ElementID = wrapper.ID;
            ParentDocumentID = wrapper.ParentDocumentID;
            Type = wrapper.Type.ToString();
            CreatedAt = wrapper.CreatedAt;
            UpdatedAt = wrapper.UpdatedAt;
            Branches = new List<Branch>();
        }

        public void SetBranches(IEnumerable<Branch> allBranches, ObjectId userID) 
        {
            if (Branches is null)
                Branches = new List<Branch>();

            foreach (Branch branch in allBranches) {
                BranchAccess access = branch.Accesses.FirstOrDefault(a => a.UserID == userID);

                if (access is {} && access.AccessType != BranchAccessType.NoAccess)
                    Branches.Add(branch);
            }
        }
    }
}