using System;
using System.Collections.Generic;
using Domain.Date;
using Domain.KernelModels;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Domain.DocumentStructureModels;

namespace Domain.Templates 
{
    public class Template : UniqueEntity
    {
        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("organizationName")]
        public string OrganizationName { get; set; }

        [BsonElement("description")]
        public string Description { get; set; }

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; }

        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; set; }

        [BsonElement("authorID")]
        public ObjectId AuthorID { get; set; }

        [BsonElement("published")]
        public bool Published { get; set; }

        [BsonElement("items")]
        public List<TemplateItem> Items { get; set; }

        [BsonElement("documentCount")]
        public long DocumentCount { get; set; }

        [BsonElement("logo")]
        public byte[] Logo { get; set; }


        public void Update(bool? newPublished, string newName, string newDescription, string newOrganization,
                           List<TemplateItem> newItems, byte[] newLogo)
        {
            if (newPublished is {})
            {
                Published = (bool)newPublished;
            }

            if (newName is {})
            {
                Name = newName;
            }

            if (newDescription is {})
            {
                Description = newDescription;
            }

            if (newOrganization is {})
            {
                OrganizationName = newOrganization;
            }

            if (newItems is {})
            {
                Items = newItems;
            }

            if (newLogo is {})
            {
                Logo = newLogo;
            }
        }


        public static Template GetEmptyTemplate(ObjectId authorID, string name, string organizationName, 
                                                string description, IDateService dateService)
        {
            DateTime creationDate = dateService.Now;
            return new Template
            {
                ID = ObjectId.GenerateNewId(),
                AuthorID = authorID,
                Name = name,
                OrganizationName = organizationName,
                Description = description,
                CreatedAt = creationDate,
                UpdatedAt = creationDate,
                Published = false,
                Items = new List<TemplateItem>()
                {
                    new TemplateItem()
                    {
                        Comment = "Enter comment here...",
                        Items = new List<TemplateItem>(),
                        ItemType = ItemType.Start,
                        Name = name,
                    }
                }
            };
        }
    }
}