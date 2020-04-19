using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Domain.DocumentElements.NumberedList
{
    public class NumberedList : DocumentElement 
    { 
        [BsonElement("elements"), JsonProperty("elements")]
        public List<NumberedListElement> Elements { get; set; }


        public static NumberedList GetDefaultList()
        {
            return new NumberedList()
            {
                Name = "Enter name here...",
                Elements = new List<NumberedListElement>()
                {
                    new NumberedListElement()
                    {
                        Content = "Enter text here...",
                        InnerElements = new List<NumberedListElement>(),
                    }
                }
            };
        }
    }
}
