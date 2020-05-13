using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Word.Interfaces;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using WordParagraph = DocumentFormat.OpenXml.Wordprocessing.Paragraph;

namespace ApplicationLib.Word.Commands
{
    public class EmptyParagraphsCommand : IWordCommand
    {
        public WordprocessingDocument WordDocument { get; }
        private int NumberOfParagraphs { get; }

        public EmptyParagraphsCommand(WordprocessingDocument wordDocument, int numberOfParagraphs)
        {
            WordDocument = wordDocument;
            NumberOfParagraphs = numberOfParagraphs;
        }

        public void Render()
        {
            for (int i = 0; i<NumberOfParagraphs; i++)
            {
                WordDocument.MainDocumentPart.Document.Body.Append(new
                    EmptyParagraphCommand().GetElement());
            }
        }
    }
}
