using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentsAPI.Models.VersionControl
{
    public class Frame
    {
        public DateTime CreatedAt { get; set; }
        public List<Change> Changes { get; set; }
    }
}
