using MongoDB.Bson;

using Newtonsoft.Json;

using Domain.JsonConverters;


namespace SentenceAPI.Features.Workplace.DocumentsStorage.Models
{
    /// <summary>
    /// This class is a model for a new folder which will be created.
    /// </summary>
    public class NewFolderDto
    {
        [JsonProperty("folderName")]
        public string FolderName { get; set; }

        [JsonProperty("parentFolderID"), JsonConverter(typeof(ObjectIDJsonConverter))]
        public ObjectId ParentFolderID { get; set; }
    }
}
