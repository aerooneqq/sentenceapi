using System;

namespace Domain.DocumentElements
{
    public class DocumentElement
    {
        public string Type { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Hint { get; set; }
        public string Name { get; set; }
    }
}
