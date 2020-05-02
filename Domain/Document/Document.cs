using System;
using System.Collections.Generic;
using Domain.Document.DocumentStatus;
using Domain.KernelModels;
using Domain.Templates;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

using Newtonsoft.Json;


namespace Domain.Models.Document
{
    public class Document : UniqueEntity
    {
        #region Properties
        [JsonProperty("authorID"), BsonElement("authorID")]
        public ObjectId AuthorID { get; set; }

        [JsonProperty("name"), BsonElement("name")]
        public string Name { get; set; }

        [JsonProperty("createdAt"), BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("updatedAt"), BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; set; }

        [JsonProperty("description"), BsonElement("description")]
        public string Description { get; set; }

        [JsonProperty("documentStatus"), BsonElement("documentStatus")]
        public DocumentStatus DocumentStatus { get; set; }
        #endregion


        #region Constructors
        public Document() { }
        public Document(ObjectId documentID, ObjectId userID, string name, DocumentType documentType,
                        DateTime creationDate)
        {
            ID = documentID;
            AuthorID = userID;
            Name = name;
            CreatedAt = creationDate;
            UpdatedAt = creationDate;
            Description = string.Empty;
            DocumentStatus = new DocumentStatus
            {
                DocumentType = documentType,
                Accesses = new List<DocumentAccess>
                {
                    new DocumentAccess(userID, DocumentUserRole.Creator, AccessType.ReadWrite)
                }
            };
        }
        #endregion
    }
}
