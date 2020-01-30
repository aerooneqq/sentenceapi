using MongoDB.Bson;

using Newtonsoft.Json;

using Domain.JsonConverters;


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
