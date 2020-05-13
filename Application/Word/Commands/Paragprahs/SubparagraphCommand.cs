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
using Paragraph = Domain.DocumentElements.Paragraph.Paragraph;
using WordParagraph = DocumentFormat.OpenXml.Wordprocessing.Paragraph;


namespace ApplicationLib.Word.Commands
{
    public class SubparagraphCommand : IWordCommand
    {
        public WordprocessingDocument WordDocument { get; }
        public Paragraph Subparagraph { get; }
        public int Depth { get; }
        private readonly RenderData renderData;

        public SubparagraphCommand(WordprocessingDocument wordDocument, Paragraph subparagraph,
            int depth, RenderData renderData)
        {
            this.renderData = renderData;
            WordDocument = wordDocument;
            Subparagraph = subparagraph;
            Depth = depth;
        }

        public void Render()
        {
            WordDocument.MainDocumentPart.Document.Body.Append(RenderSubparagraph());
        }

        private WordParagraph RenderSubparagraph()
        {
            var paragraph = new WordParagraph();
            var pp = new ParagraphProperties()
            {
                Justification = new Justification() { Val = JustificationValues.Both },
                SpacingBetweenLines = new SpacingBetweenLines()
                {
                    Before = "100",
                    After = "100",
                    Line = "300",
                    LineRule = LineSpacingRuleValues.Exact
                },
                Indentation = new Indentation() { Left = (500 * Depth).ToString() }
            };
            paragraph.Append(pp);

            var run = new Run();
            var runProperties = new RunProperties(new RunFonts
            {
                HighAnsi = new StringValue("Times New Roman"),
                Ascii = "Times New Roman"
            })
            {
                Color = new Color() { Val = renderData.RenderSettings.DefaultColor },
                FontSize = new FontSize() { Val = renderData.RenderSettings.DefaultTextSize },

            };
            run.PrependChild(runProperties);

            var text = new Text(Subparagraph.Text);

            run.Append(text);
            paragraph.Append(new TabCommand(renderData).GetElement());
            paragraph.Append(run);

            return paragraph;
        }
    }
}
