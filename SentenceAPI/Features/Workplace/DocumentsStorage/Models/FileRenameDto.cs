using MongoDB.Bson;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SharedLibrary.JsonConverters;

namespace SentenceAPI.Features.Workplace.DocumentsStorage.Models
{
    public class FileRenameDto
    {
        [JsonProperty("folderID"), JsonConverter(typeof(ObjectIDJsonConverter))]
        public ObjectId FolderID { get; set; }

        [JsonProperty("newFileName")]
        public string FileName { get; set; }
    }
}
