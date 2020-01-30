using System;

using Newtonsoft.Json;

using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

using Domain.JsonConverters;
using Domain.KernelModels;


namespace Domain.Workplace.DocumentsStorage
{
    /// <summary>
    /// This class represents the folder which can contain document 
    /// or other folders in the document file system
    /// </summary>
    public class DocumentFolder : UniqueEntity
    {
        [BsonElement("userID"), JsonProperty("userID"), JsonConverter(typeof(ObjectIDJsonConverter))]
        public ObjectId UserID { get; set; }

        [BsonElement("parentFolderID"), JsonProperty("parentFolderID"), JsonConverter(typeof(ObjectIDJsonConverter))]
        public ObjectId ParentFolderID { get; set; }
        
        [BsonElement("folderName"), JsonProperty("folderName")]
        public string FolderName { get; set; }

        [BsonElement("creationDate"), JsonProperty("creationDate")]
        public DateTime CreationDate { get; set; }

        [BsonElement("lastUpdateDate"), JsonProperty("lastUpdateDate")]
        public DateTime LastUpdateDate { get; set; }

        [BsonElement("isDeleted"), JsonProperty("isDeleted")]
        public bool IsDeleted { get; set; }
    }
}
