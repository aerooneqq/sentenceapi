using System.Collections.Generic;
using Domain.Templates;
using MongoDB.Bson;

namespace Application.Templates
{
    public class TemplateUpdateDto
    {
        public ObjectId TemplateID { get; set; }
        public string NewName { get; set; }
        public string NewDescription { get; set; }
        public string NewOrganization { get; set; }
        public List<TemplateItem> NewItems { get; set; }
        public bool? NewPublished { get; set; }

        public void Deconstruct(out bool? newPublished, out string newName, out string newDesc, out string newOrg,
                                out List<TemplateItem> newItems)
        {
            newPublished = NewPublished;
            newName = NewName;
            newDesc = NewDescription;
            newOrg = NewOrganization;
            newItems = NewItems;
        }
    }
}