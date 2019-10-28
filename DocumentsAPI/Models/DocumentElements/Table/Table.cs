using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentsAPI.Models.DocumentElements.Table
{
    public class Table : DocumentElement
    {
        public List<TableCell> Cells { get; set; }
    }
}
