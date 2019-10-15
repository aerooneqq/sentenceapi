using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentsAPI.Models.DocumentElements
{
    public enum DocumentElementType : byte
    {
        Paragraph = 0,
        Image,
        Table,
        NumberedList
    }
}
