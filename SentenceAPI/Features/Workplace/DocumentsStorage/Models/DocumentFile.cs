using DataAccessLayer.KernelModels;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Newtonsoft.Json;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SentenceAPI.Features.Workplace.DocumentsStorage.Models
{
    /// <summary>
    /// This is the document file model which represents the document file in the file system
    /// </summary>
    public class DocumentFile : UniqueEntity
    {
        [BsonElement("userID"), JsonProperty("userID")]
        public ObjectId UserID { get; set; } 
        
        [BsonElement("documentID"), JsonProperty("documentID")]
        public ObjectId DocumentID { get; set; }

        [BsonElement("parentFolderID"), JsonProperty("parentFolderID")]
        public ObjectId ParentFolderID { get; set; }

        [BsonElement("creationDate"), JsonProperty("creationDate")]
        public DateTime CreationDate { get; set; }

        [BsonElement("lastUpdatedDate"), JsonProperty("lastUpdatedDate")]
        public DateTime LastUpdateDate { get; set; }

        [BsonElement("fileName"), JsonProperty("fileName")]
        public string FileName { get; set; }
    }
}
