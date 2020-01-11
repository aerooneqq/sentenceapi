using DataAccessLayer.KernelModels;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

using Newtonsoft.Json;

using SharedLibrary.JsonConverters;

namespace SentenceAPI.Features.Workplace.DocumentsStorage.Models
{
    public class UserMainFolders : UniqueEntity
    {
        [JsonProperty("userID"), BsonElement("userID"), JsonConverter(typeof(ObjectIDJsonConverter))]
        public ObjectId UserID { get; set; }

        [JsonProperty("projectsFolderID"), BsonElement("projectsFolderID"), 
         JsonConverter(typeof(ObjectIDJsonConverter))]
        public ObjectId ProjectsFolderID { get; set; }

        [JsonProperty("localFolderID"), BsonElement("localFolderID"), 
         JsonConverter(typeof(ObjectIDJsonConverter))]
        public ObjectId LocalFolderID { get; set; }
    }
}