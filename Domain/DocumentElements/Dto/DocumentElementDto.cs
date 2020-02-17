using System;
using System.Collections.Generic;

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

        [JsonProperty("hint")]
        public string Hint { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("document")]
        public List<Branch> Branches { get; set; }
    }
}