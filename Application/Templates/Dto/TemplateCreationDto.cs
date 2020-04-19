using System.Collections.Generic;
using Domain.Templates;
using MongoDB.Bson;

namespace Application.Templates
{
    public class TemplateCreationDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Organization { get; set; }
        public ObjectId AuthorID { get; set; }
    }
}