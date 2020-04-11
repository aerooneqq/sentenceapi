using System;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Domain.DocumentElements
{
    [BsonKnownTypes(typeof(Paragraph.Paragraph), typeof(Table.Table), 
                    typeof(NumberedList.NumberedList), typeof(Image.Image))]
    public class DocumentElement
    {
        [BsonElement("type"), JsonProperty("type")]
        public string Type { get; set; }

        [BsonElement("hint"), JsonProperty("hint")]
        public string Hint { get; set; }
        
        [BsonElement("name"), JsonProperty("name")]
        public string Name { get; set; }
    }
}
