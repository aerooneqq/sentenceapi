using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentsAPI.Models.DocumentElements.NumberedList
{
    public class NumberedList : DocumentElement 
    { 
        public List<NumberedListElement> Elements { get; set; }
    }
}
