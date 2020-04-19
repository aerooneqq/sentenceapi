using System.Collections.Generic;

using Domain.KernelModels;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Domain.DocumentElements.NumberedList
{
    public class NumberedListElement
    {
        [BsonElement("elements"), JsonProperty("elements")]
        public List<NumberedListElement> InnerElements { get; set; }

        [BsonElement("content"), JsonProperty("content")]
        public string Content { get; set; }
    }
}
