using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Application.Word.Interfaces;
using Application.Word.RenderParams;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using WordParagraph = DocumentFormat.OpenXml.Wordprocessing.Paragraph;

namespace Application.Word.Commands.Items
{
    public class ItemHeaderCommand : IWordCommand
    {
        public WordprocessingDocument WordDocument { get; }
        public string Name { get; }

        private readonly RenderData renderData;

        public ItemHeaderCommand(WordprocessingDocument wordDocument, string name, RenderData renderData)
        {
            this.renderData = renderData;
            WordDocument = wordDocument;
            Name = name;
        }

        public void Render()
        {
            WordDocument.MainDocumentPart.Document.Body.Append(GetSectionHeadParagraph());
        }

        private WordParagraph GetSectionHeadParagraph()
        {
            WordParagraph p = new WordParagraph();
            ParagraphProperties pp = new ParagraphProperties(new Justification() { Val = JustificationValues.Center });

            p.Append(pp);

            Run run = new Run();
            RunProperties runProperties = new RunProperties(new RunFonts()
            {
                HighAnsi = renderData.RenderSettings.FontFamily,
                Ascii = renderData.RenderSettings.FontFamily
            })
            {
                Color = new Color() { Val = renderData.RenderSettings.DefaultColor },
                FontSize = new FontSize() { Val = renderData.RenderSettings.DefaultTextSize },
                Caps = new Caps(),
                Bold = new Bold()
            };

            run.Append(runProperties);

            Text text = new Text(Name);

            run.Append(text);
            p.Append(run);

            return p;
        }
    }
}
