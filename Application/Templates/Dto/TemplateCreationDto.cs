using System.Collections.Generic;
using Domain.JsonConverters;
using Domain.Templates;
using MongoDB.Bson;
using Newtonsoft.Json;

namespace Application.Templates
{
    public class TemplateCreationDto
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("organization")]
        public string Organization { get; set; }

        [JsonIgnore]
        public ObjectId AuthorID { get; set; }
    }
}