using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Word.Interfaces;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;
using WordParagraph = DocumentFormat.OpenXml.Wordprocessing.Paragraph;

using DocumentFormat.OpenXml.Packaging;

namespace ApplicationLib.Word.Commands
{
    public class SectionPtrCommand : IWordCommand
    {
        public WordprocessingDocument WordDocument { get; }

        public SectionPtrCommand(WordprocessingDocument wordDocument)
        {
            WordDocument = wordDocument;
        }

        public void Render()
        {
            WordDocument.MainDocumentPart.Document.Body.Append(GetSectionPtr());
        }

        public OpenXmlCompositeElement GetSectionPtr() => new WordParagraph(
                new ParagraphProperties(
                    new SectionProperties()));

    }
}
