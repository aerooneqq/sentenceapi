using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Projects;
using Domain.Users;
using MongoDB.Bson;
using Newtonsoft.Json;

namespace Application.Projects.Dto
{
    public class ProjectShortDto
    {
        [JsonProperty("id")]
        public ObjectId ID { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("updatedAt")]
        public DateTime UpdatedAt { get; set; }


        public ProjectShortDto(Project project)
        {
            ID = project.ID;
            Name = project.Name;
            Description = project.Description;
            CreatedAt = project.CreatedAt;
            UpdatedAt = project.UpdatedAt;
        }
    }
}