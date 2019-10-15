using DataAccessLayer.KernelModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentsAPI.Models.DocumentElements.NumberedList
{
    public class NumberedListElement : UniqueEntity
    {
        public object Content { get; set; }
    }
}
