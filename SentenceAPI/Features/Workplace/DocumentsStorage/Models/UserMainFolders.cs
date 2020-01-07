using DataAccessLayer.KernelModels;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace SentenceAPI.Features.Workplace.DocumentsStorage.Models
{
    public class UserMainFolders : UniqueEntity
    {
        [JsonProperty("userID"), BsonElement("userID")]
        public ObjectId UserID { get; set; }

        [JsonProperty("projectsFolderID"), BsonElement("projectsFolderID")]
        public ObjectId ProjectsFolderID { get; set; }

        [JsonProperty("localFolderID"), BsonElement("localFolderID")]
        public ObjectId LocalFolderID { get; set; }
    }
}