using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentsAPI.Models.DocumentElements.Image
{
    public class Image : DocumentElement
    {
        public byte[] Source { get; set; }
    }
}
