using System;
using System.Collections.Generic;
using Domain.JsonConverters;
using Domain.Templates;
using Domain.Users;
using MongoDB.Bson;
using Newtonsoft.Json;

namespace Application.Templates
{
    public class TemplateDto
    {
        [JsonProperty("id")]
        public ObjectId ID { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("organization")]
        public string OrganizationName { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("updatedAt")]
        public DateTime UpdatedAt { get; set; }

        [JsonProperty("author")]
        public UserDto Author { get; set; }

        [JsonProperty("published")]
        public bool Published { get; set; }

        [JsonProperty("items")]
        public List<TemplateItem> Items { get; set; }

        [JsonProperty("documentCount")]
        public long DocumentCount { get; set; }

        [JsonProperty("logo")]
        public byte[] Logo { get; set; }


        public TemplateDto(Template template, UserInfo user)
        {
            ID = template.ID;
            Author = new UserDto(user.ID, user.Login);
            Name = template.Name;
            OrganizationName = template.OrganizationName;
            Description = template.Description;
            CreatedAt = template.CreatedAt;
            UpdatedAt = template.UpdatedAt;
            Published = template.Published;
            Items = template.Items;
            DocumentCount = template.DocumentCount;
            Logo = template.Logo;
        }
    }
}