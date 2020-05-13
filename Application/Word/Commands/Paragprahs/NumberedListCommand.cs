using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Word.Interfaces;
using Application.Word.RenderParams;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;
using WordParagraph = DocumentFormat.OpenXml.Wordprocessing.Paragraph;

using DocumentFormat.OpenXml.Packaging;
using Domain.DocumentElements.NumberedList;

namespace ApplicationLib.Word.Commands
{
    public class NumberedListCommand : IWordCommand
    {
        public WordprocessingDocument WordDocument { get; }
        private int Depth { get; }
        private NumberedList NumberedList { get; }
        private readonly RenderData renderData; 

        public NumberedListCommand(WordprocessingDocument wordDocument, NumberedList numberedList,
            int depth, RenderData renderData)
        {
            this.renderData = renderData;
            Depth = depth;
            WordDocument = wordDocument;
            NumberedList = numberedList;
        }

        public void Render()
        {
            var paragraphs = RenderNumberedList();

            foreach (var p in paragraphs)
            {
                WordDocument.MainDocumentPart.Document.Body.Append(p);
            }
            renderData.CurrentNumID++;
        }
        private IEnumerable<WordParagraph> RenderNumberedList()
        {
            List<WordParagraph> wordParagraphs = new List<WordParagraph>();

            int index = 1;
            foreach (NumberedListElement element in NumberedList.Elements)
            {
                WordParagraph p = new WordParagraph();
                ParagraphProperties pp = new ParagraphProperties()
                {
                    NumberingProperties = new NumberingProperties(
                        new NumberingId() { Val = renderData.CurrentNumID},
                        new NumberingLevelReference() {Val = 0 }),
                    Indentation = new Indentation() { Left = (500 * Depth).ToString() },
                    SpacingBetweenLines = new SpacingBetweenLines()
                    {
                        Before = "100",
                        After = "100",
                        Line = "250",
                        LineRule = LineSpacingRuleValues.Exact
                    },
                };
                NumberingProperties numberingProperties = new NumberingProperties()
                {
                    NumberingLevelReference = new NumberingLevelReference() { Val = 0 },
                    NumberingId = new NumberingId() { Val = 0 }
                };

                pp.Append(numberingProperties);
                p.Append(pp);

                Run run = new Run();
                RunProperties runProperties = new RunProperties(new RunFonts() { HighAnsi = "Times New Roman" })
                {
                    FontSize = new FontSize() { Val = renderData.RenderSettings.DefaultTextSize },
                    Color = new Color() { Val = renderData.RenderSettings.DefaultColor }
                };
                Text text = new Text(index + ") " + element.Content);

                run.Append(runProperties);
                run.Append(text);

                p.Append(run);

                wordParagraphs.Add(p);
                index++;
            }

            return wordParagraphs;
        }
    }
}
