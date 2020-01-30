using MongoDB.Bson;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentenceAPI.Features.Workplace.DocumentsStorage.Models
{
    /// <summary>
    /// This class is a model for a new file creation proccess
    /// </summary>
    public class NewFileDto
    {
        private ObjectId parentFolderObjectId;
        [JsonIgnore]
        public ObjectId ParentFolderObjectId => parentFolderObjectId;
        

        private string parentFolderID;
        
        [JsonProperty("parentFolderID")]
        public string ParentFolderID { 
            get => parentFolderID;
            set
            {
                parentFolderID = value;
                parentFolderObjectId = ObjectId.Parse(value);
            }
        }
        

        [JsonProperty("fileName")]
        public string FileName { get; set; }
    }
}
