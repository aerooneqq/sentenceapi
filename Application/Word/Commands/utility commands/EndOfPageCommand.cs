using Application.Word.Interfaces;
using Application.Word.RenderParams;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;
using WordParagraph = DocumentFormat.OpenXml.Wordprocessing.Paragraph;
using DocumentFormat.OpenXml.Packaging;


namespace ApplicationLib.Word.Commands
{
    public class EndOfPageCommand : IWordCommand
    {
        public WordprocessingDocument WordDocument { get; }
        private readonly RenderData renderData;

        public EndOfPageCommand(WordprocessingDocument wordDocument, RenderData renderData)
        {
            this.renderData = renderData;
            WordDocument = wordDocument;
        }


        public void Render()
        {
            WordDocument.MainDocumentPart.Document.Body.Append(GetLastParagraphOfThePage());
        }

        public WordParagraph GetLastParagraphOfThePage ()
        {
            var paragraph = new WordParagraph();
            var pp = new ParagraphProperties()
            {
                Justification = new Justification() { Val = JustificationValues.Center },
            };
            paragraph.Append(pp);

            var run = new Run(new LastRenderedPageBreak(), new Break() { Type = BreakValues.Page });
            var runProperties = new RunProperties(new RunFonts
            {
                HighAnsi = new StringValue(
                renderData.RenderSettings.FontFamily)
            })
            {
                Color = new Color() { Val = renderData.RenderSettings.DefaultColor },
                FontSize = new FontSize() { Val = "30" }
            };
            run.PrependChild(runProperties);

            var wordText = new Text();

            run.Append(wordText);
            paragraph.Append(run);

            return paragraph;
        }
    }
}
