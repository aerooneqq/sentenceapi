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
    public class ParagraphHeaderCommand : IWordCommand
    {
        public WordprocessingDocument WordDocument { get; }
        public string Name { get;  }
        public int Depth { get; }
        private readonly RenderData renderData;

        public ParagraphHeaderCommand(WordprocessingDocument document, string name, int depth, RenderData renderData)
        {
            this.renderData = renderData;
            WordDocument = document;
            Name = name;
            Depth = depth;
        }
        public void Render()
        {
            WordDocument.MainDocumentPart.Document.Body.Append(GetParagraphHeader());
        }

        private WordParagraph GetParagraphHeader()
        {
            WordParagraph p = new WordParagraph();
            ParagraphProperties pp = new ParagraphProperties(
                new Justification() { Val = JustificationValues.Left },
                new Indentation() { Left = (renderData.RenderSettings.TabValue * Depth).ToString() });

            p.Append(pp);

            Run run = new Run();
            RunProperties runProperties = new RunProperties(new RunFonts()
            { HighAnsi = renderData.RenderSettings.FontFamily, Ascii = renderData.RenderSettings.FontFamily })
            {
                Color = new Color() { Val = renderData.RenderSettings.DefaultColor },
                FontSize = new FontSize() { Val = renderData.RenderSettings.DefaultTextSize },
                Bold = new Bold()
            };

            Text text = new Text(Name);

            run.Append(runProperties);
            run.Append(text);
            p.Append(run);

            return p;
        }
    }
}
