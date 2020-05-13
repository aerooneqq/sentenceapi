using DocumentFormat.OpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Word.Interfaces
{
    /// <summary>
    /// The secondary command is used in IWordCommand to get small elements, like a Tab run and so on
    /// </summary>
    public interface IWordSecondaryCommand
    {
        OpenXmlCompositeElement GetElement();
    }
}
