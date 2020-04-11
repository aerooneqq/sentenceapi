using Domain.VersionControl;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

using Newtonsoft.Json;

using System.Collections.Generic;

using Domain.KernelModels;
using System;

namespace Domain.DocumentElements
{
    public class DocumentElementWrapper : UniqueEntity
    {
        [BsonElement("parentDocumentID"), JsonProperty("parentDocumentID")]
        public ObjectId ParentDocumentID { get; set; }

        [BsonElement("parentItemID"), JsonProperty("parentItemID")]
        public ObjectId ParentItemID { get; set; }

        [BsonElement("creatorID"), JsonProperty("creatorID")]
        public ObjectId CreatorID { get; set; }

        [BsonElement("documentElementType"), JsonProperty("documentElementType")]
        public DocumentElementType Type { get; set; }

        [BsonElement("isDeleted"), JsonProperty("isDeleted")]
        public bool IsDeleted { get; set; }

        [BsonElement("createdAt"), JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }
        
        [BsonElement("updatedAt"), JsonProperty("updatedAt")]
        public DateTime UpdatedAt { get; set; }

        [BsonElement("currentBranchID"), JsonProperty("currentBranchID")]
        public ObjectId CurrentBranchID { get; set; }

        [BsonElement("currentBranchNodeID"), JsonProperty("currentBranchNodeID")]
        public ObjectId CurrentBranchNodeID { get; set; }
        

        /// <summary>
        /// The versions of the document element
        /// </summary>
        [BsonElement("branches"), JsonIgnore]
        public List<Branch> Branches { get; set; }
    }
}
