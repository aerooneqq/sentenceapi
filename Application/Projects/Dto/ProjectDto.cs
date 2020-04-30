using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Projects;
using Domain.Users;
using MongoDB.Bson;
using Newtonsoft.Json;

namespace Application.Projects.Dto
{
    public class ProjectDto
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

        [JsonProperty("users")]
        public List<ProjectUserDto> Users { get; set; }

        [JsonProperty("projectDocuments")]
        public List<ProjectDocumentDto> ProjectDocuments { get; set; }


        public ProjectDto(Project project, IDictionary<ObjectId, UserInfo> users,
                          IDictionary<ObjectId, byte[]> userPhotos)
        {
            ID = project.ID;
            Name = project.Name;
            Description = project.Description;
            CreatedAt = project.CreatedAt;
            UpdatedAt = project.UpdatedAt;
            Users = project.Users.Select(user => new ProjectUserDto()
            {
                AuthorName = users[user.UserID].Name,
                Role = user.Role,
                UserID = user.UserID,
                UserPhoto = userPhotos[user.UserID],
            }).ToList();
            ProjectDocuments = new List<ProjectDocumentDto>();
        }
    }
}