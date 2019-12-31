using DataAccessLayer.KernelModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentsAPI.Models.Document
{
    public class Document : UniqueEntity
    {
        public long AuthorID { get; set; }

        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Description { get; set; }
        public List<long> FileIDs { get; set; }

        public DocumentType DocumentType { get; set; }
    }
}
