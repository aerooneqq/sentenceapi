using System;
using System.Collections.Generic;

using MongoDB.Bson;


namespace Domain.VersionControl
{
    public class Branch
    {
        public ObjectId Author { get; set; }
        public string BranchName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; } 
        public List<BranchAccess> Accesses { get; set; }
        public List<BranchNode> BranchNodes { get; set; }
    }
}