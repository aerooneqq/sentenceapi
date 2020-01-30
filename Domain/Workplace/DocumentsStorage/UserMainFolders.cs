using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

using Newtonsoft.Json;

using Domain.JsonConverters;
using Domain.KernelModels;


namespace Domain.Workplace.DocumentsStorage
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