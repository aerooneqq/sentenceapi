using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Word.Interfaces;
using DocumentFormat.OpenXml.Wordprocessing;
using WordParagraph = DocumentFormat.OpenXml.Wordprocessing.Paragraph;

using DocumentFormat.OpenXml;

namespace ApplicationLib.Word.Commands
{
    public class EmptyParagraphCommand : IWordSecondaryCommand
    {
        public OpenXmlCompositeElement GetElement()
        {
            var paragraph = new WordParagraph();
            var pp = new ParagraphProperties()
            {
                Justification = new Justification()
                {
                    Val = JustificationValues.Center
                }
            };
            paragraph.Append(pp);
            var run = new Run();
            var runProperties = new RunProperties();
            run.PrependChild(runProperties);
            paragraph.Append(run);

            return paragraph;
        }
    }
}
