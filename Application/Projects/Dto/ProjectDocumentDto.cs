using System;
using System.Collections.Generic;
using Domain.Document.DocumentStatus;
using MongoDB.Bson;
using Newtonsoft.Json;

namespace Application.Projects.Dto
{
    public class ProjectDocumentDto
    {
        [JsonProperty("documentID")]
        public ObjectId DocumentID { get; set; }

        [JsonProperty("documentName")]
        public string DocumentName { get; set; }

        [JsonProperty("documentStatus")]
        public DocumentStatus DocumentStatus { get; set; }
    }
}