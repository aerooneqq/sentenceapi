using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentenceAPI.Features.Workplace.DocumentsStorage.Models
{
    public class FileRenameDto
    {
        [JsonProperty("folderID")]
        public long FolderID { get; set; }

        [JsonProperty("newFileName")]
        public string FileName { get; set; }
    }
}
