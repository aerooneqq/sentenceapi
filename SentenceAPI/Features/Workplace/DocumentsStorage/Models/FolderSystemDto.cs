using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentenceAPI.Features.Workplace.DocumentsStorage.Models
{
    /// <summary>
    /// Data transfer object which is used to return the the file system.
    /// </summary>
    public class FolderSystemDto
    {
        #region Properties
        [JsonProperty("files")]
        public IEnumerable<DocumentFile> Files { get; set; }

        [JsonProperty("folders")]
        public IEnumerable<DocumentFolder> Folders { get; set; }
        #endregion

        #region Constructors
        public FolderSystemDto() { }

        public FolderSystemDto(IEnumerable<DocumentFile> files, IEnumerable<DocumentFolder> folders)
        {
            Files = files;
            Folders = folders;
        }
        #endregion

    }
}
