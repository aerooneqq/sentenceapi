using System;
using Domain.Projects;
using MongoDB.Bson;
using Newtonsoft.Json;

namespace Application.Projects.Dto
{
    public class ProjectUserDto
    {
        [JsonProperty("userID")]
        public ObjectId UserID { get; set; }

        [JsonProperty("role")]
        public ProjectRole Role { get; set; }

        [JsonProperty("authorName")]
        public string AuthorName { get; set; }

        [JsonProperty("userPhoto")]
        public byte[] UserPhoto { get; set; }
    }
}