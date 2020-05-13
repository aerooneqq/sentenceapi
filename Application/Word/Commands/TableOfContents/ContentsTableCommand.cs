using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Word.Dto;
using Application.Word.Interfaces;
using Application.Word.RenderParams;
using WordParagraph = DocumentFormat.OpenXml.Wordprocessing.Paragraph;
using WordTable = DocumentFormat.OpenXml.Wordprocessing.Table;
using WordDocument = DocumentFormat.OpenXml.Wordprocessing.Document;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml;

using DocumentFormat.OpenXml.Packaging;

namespace ApplicationLib.Word.Commands
{
    public class ContentsTableCommand : IWordCommand
    {
        public WordprocessingDocument WordDocument { get; }
        private readonly RenderData renderData;
        public ContentsTableCommand(WordprocessingDocument document, RenderData renderData)
        {
            this.renderData = renderData;
            WordDocument = document;
        }

        public void Render()
        {
            WordDocument.MainDocumentPart.Document.Body.Append(GetTableOfContents());
        }

        private SdtBlock GetTableOfContents()
        {
            SdtBlock tableOfContents = new SdtBlock();
            RunProperties tocRpr = new RunProperties(new RunFonts()
            { HighAnsi = renderData.RenderSettings.FontFamily },
                new Color() { Val = "auto" }, new FontSize() { Val = renderData.RenderSettings.DefaultTextSize });
            SdtContentDocPartObject sdtContentDocPartObject = new SdtContentDocPartObject(
                new DocPartGallery() { Val = "Table of Contents" }, new DocPartUnique());

            SdtProperties sdtProperties = new SdtProperties(tocRpr, sdtContentDocPartObject);

            tableOfContents.Append(sdtProperties);
            tableOfContents.Append(new SdtEndCharProperties());

            SdtContentBlock sdtContentBlock = new SdtContentBlock();

            WordParagraph p = new WordParagraph();
            ParagraphProperties ppr = new ParagraphProperties()
            {
                Justification = new Justification() { Val = JustificationValues.Center }
            };

            p.Append(ppr);

            RunProperties rpr = new RunProperties(new RunFonts()
            {
                Ascii = renderData.RenderSettings.FontFamily,
                HighAnsi = renderData.RenderSettings.FontFamily
            })
            {
                Bold = new Bold(),
                Caps = new Caps(),
                FontSize = new FontSize() { Val = renderData.RenderSettings.DefaultTextSize }
            };

            Run run = new Run();
            run.Append(rpr);

            Text text = new Text("Содержание");
            run.Append(text);

            p.Append(run);

            sdtContentBlock.Append(p);

            string index = "0";
            foreach (DocumentElementRenderDto element in renderData.Document.Elements)
            {
                index = (int.Parse(index) + 1).ToString();
                UploadItemsToTableOfContents(element, sdtContentBlock, 0, index);
            }

            tableOfContents.Append(sdtContentBlock);

            return tableOfContents;
        }

        private void UploadItemsToTableOfContents(DocumentElementRenderDto element, SdtContentBlock sdtContentBlock,
            int depth, string index)
        {
            AddTableOfContentsElement(sdtContentBlock, depth, index + ". " + element.Name);

            string dopIndex = "0";
            if (element.InnerElements is {})
            {
                foreach (DocumentElementRenderDto i in element.InnerElements)
                {
                    dopIndex = (int.Parse(dopIndex) + 1).ToString();
                    UploadItemsToTableOfContents(i, sdtContentBlock, depth + 1, index + "." + dopIndex);
                }
            }
        }

        private void AddTableOfContentsElement(SdtContentBlock sdtContentBlock, int depth,
           string name)
        {
            WordParagraph p = new WordParagraph();
            ParagraphProperties pp = new ParagraphProperties(new ParagraphStyleId() { Val = "31" })
            {
                Indentation = new Indentation() { FirstLine = "262", Left = "0" },
                Justification = new Justification() { Val = JustificationValues.Both }
            };

            RunFonts runFonts = new RunFonts()
            {
                Ascii = renderData.RenderSettings.FontFamily,
                HighAnsi = renderData.RenderSettings.FontFamily
            };
            pp.Append(runFonts);

            p.Append(pp);

            for (int i = 0; i < depth; i++)
            {
                p.Append(new TabCommand(renderData).GetElement());
            }

            Run run = new Run();
            RunProperties runProperties = new RunProperties(new RunFonts()
            {
                HighAnsi = renderData.RenderSettings.FontFamily,
                Ascii = renderData.RenderSettings.FontFamily
            })
            {
                FontSize = new FontSize() { Val = renderData.RenderSettings.DefaultTextSize }
            };
            if (depth == 0)
            {
                runProperties.Bold = new Bold();
            }
            Text text = new Text(name);

            run.Append(runProperties);
            run.Append(text);

            p.Append(run);

            run = new Run();
            runProperties = new RunProperties(new RunFonts()
            {
                HighAnsi = renderData.RenderSettings.FontFamily,
                Ascii = renderData.RenderSettings.FontFamily
            });
            run.Append(runProperties);
            run.Append(new PositionalTab()
            {
                Leader = AbsolutePositionTabLeaderCharValues.Dot,
                Alignment = AbsolutePositionTabAlignmentValues.Right,
                RelativeTo = AbsolutePositionTabPositioningBaseValues.Margin
            });

            p.Append(run);

            run = new Run();
            runProperties = new RunProperties(new RunFonts()
            {
                HighAnsi = renderData.RenderSettings.FontFamily,
                Ascii = renderData.RenderSettings.FontFamily
            });
            run.Append(runProperties);

            p.Append(run);

            sdtContentBlock.Append(p);
        }
    }
}
