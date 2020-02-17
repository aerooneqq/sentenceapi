using Domain.VersionControl;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

using Newtonsoft.Json;

using System.Collections.Generic;

using Domain.KernelModels;


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

        [BsonElement("versionsCount"), JsonProperty("versionsCount")]
        public int VersionsCount { get; set; }

        [BsonElement("documentElementType"), JsonProperty("documentElementType")]
        public DocumentElementType Type { get; set; }

        /// <summary>
        /// The versions of the document element
        /// </summary>
        [BsonElement("branches"), JsonIgnore]
        public List<Branch> Branches { get; set; }
    }
}
