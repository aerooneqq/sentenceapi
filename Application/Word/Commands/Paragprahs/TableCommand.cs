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
using WordTable = DocumentFormat.OpenXml.Wordprocessing.Table;

using DocumentFormat.OpenXml.Packaging;

namespace ApplicationLib.Word.Commands
{
    public class TableCommand : IWordCommand
    {
        public WordprocessingDocument WordDocument { get; }
        private Domain.DocumentElements.Table.Table Table { get; }
        private int Depth { get; }
        private readonly RenderData renderData;

        public TableCommand(WordprocessingDocument wordDocument, Domain.DocumentElements.Table.Table table,
            int depth, RenderData renderData)
        {
            this.renderData = renderData;
            Depth = depth;
            WordDocument = wordDocument;
            Table = table;
        }

        public void Render()
        {
            WordDocument.MainDocumentPart.Document.Body.Append(GetTableNumberParagraph());
            WordDocument.MainDocumentPart.Document.Body.Append(GetTableNameParagraph());

            WordTable wordTable = RenderTable();
            WordDocument.MainDocumentPart.Document.Body.Append(wordTable);
        }

        private WordParagraph GetTableNumberParagraph()
        {
            var paragraph = new WordParagraph();
            var pp = new ParagraphProperties()
            {
                Justification = new Justification() { Val = JustificationValues.Right },
                SpacingBetweenLines = new SpacingBetweenLines()
                {
                    Before = "100",
                    After = "100",
                    Line = "300",
                    LineRule = LineSpacingRuleValues.Exact
                }
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
                FontSize = new FontSize()
                {
                    Val = renderData.RenderSettings.DefaultTextSize
                },

            };
            run.PrependChild(runProperties);

            var text = new Text("Таблица (номер)");

            run.Append(text);
            paragraph.Append(run);

            return paragraph;
        }

        private WordParagraph GetTableNameParagraph()
        {
            var paragraph = new WordParagraph();
            var pp = new ParagraphProperties()
            {
                Justification = new Justification() { Val = JustificationValues.Center },
                SpacingBetweenLines = new SpacingBetweenLines()
                {
                    Before = "100",
                    After = "100",
                    Line = "300",
                    LineRule = LineSpacingRuleValues.Exact
                }
            };
            paragraph.Append(pp);

            var run = new Run();
            var runProperties = new RunProperties(new RunFonts
            {
                HighAnsi = new StringValue(renderData.RenderSettings.FontFamily),
                Ascii = renderData.RenderSettings.FontFamily
            })
            {
                Color = new Color() { Val = renderData.RenderSettings.DefaultColor },
                FontSize = new FontSize()
                {
                    Val = renderData.RenderSettings.DefaultTextSize
                },

            };
            run.PrependChild(runProperties);

            var text = new Text(Table.Name);

            run.Append(text);
            paragraph.Append(run);

            return paragraph;
        }

        private WordTable RenderTable()
        {
            WordTable wordTable = new WordTable();
            TableProperties tableProperties = new TableProperties(new TableBorders(
                new TopBorder
                {
                    Val = new EnumValue<BorderValues>(BorderValues.Single),
                    Size = 6
                },
                new BottomBorder
                {
                    Val = new EnumValue<BorderValues>(BorderValues.Single),
                    Size = 6
                },
                new LeftBorder
                {
                    Val = new EnumValue<BorderValues>(BorderValues.Single),
                    Size = 6
                },
                new RightBorder
                {
                    Val = new EnumValue<BorderValues>(BorderValues.Single),
                    Size = 6
                },
                new InsideHorizontalBorder
                {
                    Val = new EnumValue<BorderValues>(BorderValues.Single),
                    Size = 6
                },
                new InsideVerticalBorder
                {
                    Val = new EnumValue<BorderValues>(BorderValues.Single),
                    Size = 6
                }));

            wordTable.Append(tableProperties);

            var data = Table.Cells;

            for (int i = 0; i < data.Count; i++)
            {
                TableRow tableRow = new TableRow();
                tableRow.Append(new TableRowProperties(new TableRowHeight() { Val = 100, HeightType = HeightRuleValues.Auto }));

                for (int j = 0; j < data[i].Count; j++)
                {
                    TableCell tableCell = new TableCell();

                    if (j == 3)
                    {
                        tableCell.Append(new TableCellProperties(new HorizontalMerge() { Val = MergedCellValues.Restart }));
                    }
                    else if (j == 4)
                    {
                        tableCell.Append(new TableCellProperties(new HorizontalMerge() { Val = MergedCellValues.Continue }));
                    }


                    WordParagraph p = new WordParagraph();
                    ParagraphProperties pp = new ParagraphProperties(new Justification()
                    { Val = JustificationValues.Center },
                    new SpacingBetweenLines()
                    {
                        After = "0"
                    });

                    p.Append(pp);

                    Run run = new Run();
                    RunProperties rp = new RunProperties(new RunFonts()
                    {
                        HighAnsi = "Times New Roman",
                        Ascii = "Times New Roman"
                    })
                    {
                        Color = new Color() { Val = renderData.RenderSettings.DefaultColor },
                        FontSize = new FontSize() { Val = renderData.RenderSettings.DefaultTextSize }
                    };

                    run.Append(rp);

                    Text text = new Text(data[i][j].Content);
                    run.Append(text);
                    p.Append(run);

                    tableCell.Append(p);

                    tableRow.Append(tableCell);
                }

                wordTable.Append(tableRow);
            }

            return wordTable;
        }
    }
}
