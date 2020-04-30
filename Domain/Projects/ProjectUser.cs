using System;
using Domain.KernelModels;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Projects
{
    public class ProjectUser
    {
        [BsonElement("userID")]
        public ObjectId UserID { get; set; }

        [BsonElement("role")]
        public ProjectRole Role { get; set; }
    }
}