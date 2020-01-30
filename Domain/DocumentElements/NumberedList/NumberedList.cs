using System.Collections.Generic;

namespace Domain.DocumentElements.NumberedList
{
    public class NumberedList : DocumentElement 
    { 
        public List<NumberedListElement> Elements { get; set; }
    }
}
