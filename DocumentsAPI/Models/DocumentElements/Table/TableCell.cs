using DataAccessLayer.KernelModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentsAPI.Models.DocumentElements.Table
{
    public class TableCell : UniqueEntity
    {
        public object Content { get; set; }
        public long ParentTableID { get; set; }
    }
}
