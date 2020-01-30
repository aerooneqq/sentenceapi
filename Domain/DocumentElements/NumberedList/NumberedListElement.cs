using System.Collections.Generic;

using Domain.KernelModels;


namespace Domain.DocumentElements.NumberedList
{
    public class NumberedListElement : UniqueEntity
    {
        public List<NumberedListElement> InnerElements { get; set; }
        public string Content { get; set; }
    }
}
