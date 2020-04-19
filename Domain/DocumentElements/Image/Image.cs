using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Domain.DocumentElements.Image
{
    public class Image : DocumentElement
    {
        [BsonElement("source"), JsonProperty("source")]
        public byte[] Source { get; set; }
    }
}
