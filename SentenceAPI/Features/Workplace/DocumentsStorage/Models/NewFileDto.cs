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
        [JsonProperty("parentFolderID")]
        public ObjectId ParentFolderID { get; set; }

        [JsonProperty("fileName")]
        public string FileName { get; set; }
    }
}
