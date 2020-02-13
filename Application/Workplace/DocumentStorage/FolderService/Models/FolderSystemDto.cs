using Newtonsoft.Json;

using System.Collections.Generic;
using Domain.Workplace.DocumentsStorage;


namespace Application.Workplace.DocumentStorage.FolderService.Models
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
