using MongoDB.Bson;

using Newtonsoft.Json;

using Domain.JsonConverters;


namespace Application.Workplace.DocumentStorage.FileService.Models
{
    public class FileRenameDto
    {
        [JsonProperty("folderID"), JsonConverter(typeof(ObjectIDJsonConverter))]
        public ObjectId FolderID { get; set; }

        [JsonProperty("newFileName")]
        public string FileName { get; set; }
    }
}
