using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Word.Containers;
using Application.Word.Interfaces;
using Application.Word.RenderParams;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;
using WordParagraph = DocumentFormat.OpenXml.Wordprocessing.Paragraph;
using WordTable = DocumentFormat.OpenXml.Wordprocessing.Table;
using WordDocument = DocumentFormat.OpenXml.Wordprocessing.Document;

using DocumentFormat.OpenXml.Packaging;


namespace ApplicationLib.Word.Commands
{
    public class TableOfContentsPageCommand : IWordCommand
    {
        public WordprocessingDocument WordDocument { get; }
        public ICommandsContainer CommandsContainer { get; private set; }

        private readonly RenderData renderData;

        public TableOfContentsPageCommand(WordprocessingDocument document, RenderData renderData)
        {
            this.renderData = renderData;
            CommandsContainer = new CommandsContainer();
            WordDocument = document;
        }

        public void Render()
        {
            CreateCommandsList();

            CommandsContainer.Render();
        }

        private void CreateCommandsList()
        {
            CommandsContainer.Refresh();

            CommandsContainer.Add(new ContentsTableCommand(WordDocument, renderData));
            CommandsContainer.Add(new EmptyParagraphsCommand(WordDocument, 1));
            CommandsContainer.Add(new EndOfPageCommand(WordDocument, renderData));
        }
    }
}
