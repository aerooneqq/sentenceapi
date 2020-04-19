using System;
using System.Collections.Generic;
using Domain.Date;
using Domain.KernelModels;
using MongoDB.Bson;

namespace Domain.Templates 
{
    public class Template : UniqueEntity
    {
        public string Name { get; set; }
        public string OrganizationName { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ObjectId AuthorId { get; set; }
        public bool Published { get; set; }
        public List<TemplateItem> Items { get; set; }
        public long DocumentCount { get; set; }


        public void Update(bool? newPublished, string newName, string newDescription, string newOrganization,
                           List<TemplateItem> newItems)
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
        }


        public static Template GetEmptyTemplate(ObjectId authorID, string name, string organizationName, 
                                                string description, IDateService dateService)
        {
            DateTime creationDate = dateService.Now;
            return new Template
            {
                ID = ObjectId.GenerateNewId(),
                AuthorId = authorID,
                Name = name,
                OrganizationName = organizationName,
                Description = description,
                CreatedAt = creationDate,
                UpdatedAt = creationDate,
                Published = false,
                Items = new List<TemplateItem>(),
            };
        }
    }
}