using Domain.KernelModels;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Domain.Links
{
    public class WordDownloadLink : UniqueEntity
    {
        [BsonElement("documentID"), JsonProperty("documentID")] 
        public ObjectId DocumentID { get; set; }
        
        [BsonElement("userID"), JsonProperty("userID")]
        public ObjectId UserID { get; set; }
        
        [BsonElement("used"), JsonProperty("used")]
        public bool Used { get; set; }


        public WordDownloadLink(ObjectId userID, ObjectId documentID)
        {
            DocumentID = documentID;
            UserID = userID;
            ID = ObjectId.GenerateNewId();
        }
    }
}