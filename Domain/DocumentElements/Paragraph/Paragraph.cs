using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Domain.DocumentElements.Paragraph
{
    public class Paragraph : DocumentElement
    {
        [BsonElement("text"), JsonProperty("text")]
        public string Text { get; set; }

        public Paragraph() 
        {
            Text = "Enter some text here...";
            Name = "Enter name here...";
        }
    }
}
