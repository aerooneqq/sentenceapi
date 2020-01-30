using System; 
using System.Collections.Generic;

using Domain.Date;
using Domain.KernelModels;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

using Newtonsoft.Json;


namespace Domain.DocumentStructureModels
{ 
    public class DocumentStructureModel : UniqueEntity
    { 
        [BsonElement("lastUpdatedAt"), JsonProperty("lastUpdatedAt")]
        public DateTime LastUpdatedAt { get; set; }

        [BsonElement("parentDocumentID"), JsonProperty("parentDocumentID")]
        public ObjectId ParentDocumentID { get; set; }

        [BsonElement("items"), JsonProperty("items")]
        public List<Item> Items { get; set; }


        public static DocumentStructureModel GetNewDocumentStructure(DateTime lastUpdatedDate, ObjectId documentID) =>
            new DocumentStructureModel
            {
                ID = ObjectId.GenerateNewId(),
                Items = new List<Item>(),
                LastUpdatedAt = lastUpdatedDate,
                ParentDocumentID = documentID
            };
    }
}