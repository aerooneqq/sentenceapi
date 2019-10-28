using DataAccessLayer.KernelModels;
using DocumentsAPI.Models.VersionControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentsAPI.Models.DocumentElements
{
    class DocumentElement
    {
        public string Type { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Hint { get; set; }
        public string Name { get; set; }
    }
}
