using System;
using Domain.JsonConverters;
using Domain.Projects;
using MongoDB.Bson;
using Newtonsoft.Json;

namespace Application.Projects.Dto
{
    public class ProjectUpdateDto
    {
        [JsonProperty("projectID"), JsonConverter(typeof(ObjectIDJsonConverter))]
        public ObjectId ProjectID { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }
}