using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Application.Word.Interfaces
{
    /// <summary>
    /// IWordCommand is used to create a part of a document like tables, paragraphs and so on.
    /// IWordCommand can be used singly or it can be used in a IWordMainCommand
    /// </summary>
    public interface IWordCommand
    {
        void Render();
    }
}
