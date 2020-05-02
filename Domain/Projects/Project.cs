using System;
using System.Collections.Generic;
using Domain.Date;
using Domain.KernelModels;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Projects
{
    public class Project : UniqueEntity
    {
        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("description")]
        public string Description { get; set; }

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; }

        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; set; }

        [BsonElement("users")]
        public List<ProjectUser> Users { get; set; }

        [BsonElement("documents")]
        public List<ObjectId> Documents { get; set; }

        [BsonElement("invitedUsers")]
        public List<ObjectId> InvitedUsers { get; set; }


        public static Project GetEmptyProject(ObjectId authorID, string name, string description,
                                              IDateService dateService)
        {
            return new Project()
            {
                CreatedAt = dateService.Now,
                Description = description,
                Documents = new List<ObjectId>(),
                ID = ObjectId.GenerateNewId(),
                Name = name,
                UpdatedAt = dateService.Now,
                Users = new List<ProjectUser>()
                {
                    new ProjectUser()
                    {
                        Role = ProjectRole.Creator,
                        UserID = authorID,
                    }
                },
                InvitedUsers = new List<ObjectId>(),
            };
        }
    }
}