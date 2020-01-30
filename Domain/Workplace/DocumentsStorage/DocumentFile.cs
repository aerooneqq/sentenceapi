using System;

using Newtonsoft.Json;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

using Domain.JsonConverters;
using Domain.KernelModels;


namespace Domain.Workplace.DocumentsStorage
{
    /// <summary>
    /// This is the document file model which represents the document file in the file system
    /// </summary>
    public class DocumentFile : UniqueEntity
    {
        [BsonElement("userID"), JsonProperty("userID"), JsonConverter(typeof(ObjectIDJsonConverter))]
        public ObjectId UserID { get; set; } 
        
        [BsonElement("documentID"), JsonProperty("documentID"), JsonConverter(typeof(ObjectIDJsonConverter))]
        public ObjectId DocumentID { get; set; }

        [BsonElement("parentFolderID"), JsonProperty("parentFolderID"), JsonConverter(typeof(ObjectIDJsonConverter))]
        public ObjectId ParentFolderID { get; set; }

        [BsonElement("creationDate"), JsonProperty("creationDate")]
        public DateTime CreationDate { get; set; }

        [BsonElement("lastUpdatedDate"), JsonProperty("lastUpdatedDate")]
        public DateTime LastUpdateDate { get; set; }

        [BsonElement("fileName"), JsonProperty("fileName")]
        public string FileName { get; set; }
    }
}
