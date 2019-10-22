using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentsAPI.Models.Document
{
    public enum DocumentType : byte
    {
        UserOnly = 0,
        Project,
        Shared,
    }
}
