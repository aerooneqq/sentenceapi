using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Word.Containers;
using Application.Word.Dto;
using Application.Word.Interfaces;
using Application.Word.RenderParams;
using ApplicationLib.Word.Commands;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Domain.DocumentElements;
using Domain.DocumentElements.NumberedList;
using WordParagraph = DocumentFormat.OpenXml.Wordprocessing.Paragraph;

namespace Application.Word.Commands.Items
{
    public class ItemsCommand : IWordCommand
    {
        public WordprocessingDocument WordDocument { get; }
        public ICommandsContainer CommandsContainer { get;  }

        private readonly RenderData renderData;
            
        public ItemsCommand(WordprocessingDocument wordDocument, RenderData renderData)
        {
            this.renderData = renderData;
            WordDocument = wordDocument;
            CommandsContainer = new CommandsContainer();
        }

        public void Render()
        {
            CreateCommandsList();

            CommandsContainer.Render();
        }

        private void CreateCommandsList()
        {
            CommandsContainer.Refresh();
            string index = "0";

            foreach (DocumentElementRenderDto element in renderData.Document.Elements)
            {
                index = (int.Parse(index) + 1).ToString();
                AddCommandsToList(element, 0, index);
                CommandsContainer.Add(new EndOfPageCommand(WordDocument, renderData));
            }

            CommandsContainer.Add(new SectionPtrCommand(WordDocument));
        }

        private void AddCommandsToList(DocumentElementRenderDto element, int depth, string index)
        {
            if (depth == 0)
                CommandsContainer.Add(new ItemHeaderCommand(WordDocument, index + ". " + element.Name, renderData));
            else
                CommandsContainer.Add(new ParagraphHeaderCommand(WordDocument, index + " " + element.Name, depth, renderData));

            if (element.InnerElements is {})
            {
                string dopIndex = "0";
                foreach (DocumentElementRenderDto el in element.InnerElements)
                {
                    dopIndex = (int.Parse(dopIndex) + 1).ToString();
                    AddCommandsToList(el, depth + 1, index + "." + dopIndex);
                }
            }

            if (element.Elements is {})
            {
                foreach (DocumentElement documentElement in element.Elements)
                {
                    switch (documentElement)
                    {
                        case Domain.DocumentElements.Table.Table table:
                            CommandsContainer.Add(new TableCommand(WordDocument, table, depth, renderData));
                            break;
                        case Domain.DocumentElements.Paragraph.Paragraph paragraph:
                            CommandsContainer.Add(new SubparagraphCommand(WordDocument, paragraph, depth, renderData));
                            break;
                        case NumberedList list:
                            CommandsContainer.Add(new NumberedListCommand(WordDocument, list, depth, renderData));
                            break;
                        case Domain.DocumentElements.Image.Image image:
                            CommandsContainer.Add(new ParagraphImageCommand(WordDocument, image, depth, renderData));
                            break;
                    }

                    CommandsContainer.Add(new EmptyParagraphsCommand(WordDocument, 1));
                }
            }
        }
    }
}
