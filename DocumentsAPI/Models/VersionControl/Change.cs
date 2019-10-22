using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentsAPI.Models.VersionControl
{
    public class Change
    {
        public ActionType ActionType { get; set; }
        public long StartPos { get; set; }
        public long AffectedBytesCount { get; set; }
        public byte[] Delta { get; set; }
    }
}
