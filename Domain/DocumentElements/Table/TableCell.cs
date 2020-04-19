using Domain.KernelModels;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Domain.DocumentElements.Table
{
    public class TableCell
    {
        [BsonElement("content"), JsonProperty("content")]
        public string Content { get; set; }
    }
}
