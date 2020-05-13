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


namespace ApplicationLib.Word.Commands
{
    public class RightParagraphCommand : IWordSecondaryCommand
    {
        public string Text { get; }
        private readonly RenderData renderData;

        public RightParagraphCommand(string text)
        {
            Text = text;
        }

        public OpenXmlCompositeElement GetElement()
        {
            var paragraph = new WordParagraph();
            var pp = new ParagraphProperties()
            {
                Justification = new Justification() { Val = JustificationValues.Right },
                SpacingBetweenLines = new SpacingBetweenLines()
                {
                    Before = "100",
                    After = "100",
                    Line = "250",
                    LineRule = LineSpacingRuleValues.Exact
                }
            };
            paragraph.Append(pp);

            var run = new Run();
            var runProperties = new RunProperties(new RunFonts
            {
                HighAnsi = new StringValue(renderData.RenderSettings.FontFamily)
            })
            {
                Color = new Color() { Val = renderData.RenderSettings.DefaultColor },
                FontSize = new FontSize() { Val = "27" },

            };
            run.PrependChild(runProperties);

            var wordText = new Text(Text);

            run.Append(wordText);
            paragraph.Append(run);

            return paragraph;
        }
    }
}
