using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;
using Application.Word.Commands.Items;
using Application.Word.Containers;
using Domain.Models.Document;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

using WordParagraph = DocumentFormat.OpenXml.Wordprocessing.Paragraph;
using WordTable = DocumentFormat.OpenXml.Wordprocessing.Table;
using WordDocument = DocumentFormat.OpenXml.Wordprocessing.Document;

using Application.Word.Interfaces;
using ApplicationLib.Word.Commands;
using Application.Word.Dto;
using Application.Word.RenderParams;

namespace Application.Word
{
    public class WordRenderer : IDocumentRenderer
    {
        public ICommandsContainer CommandsContainer { get; set; }
        private WordprocessingDocument WordDocument { get; set; }
        private readonly RenderData renderData;

        public WordRenderer()
        {
            CommandsContainer = new CommandsContainer();
        }

        public WordRenderer(RenderSettings renderSettings, Domain.Models.Document.Document document,
                                 List<DocumentElementRenderDto> elements) : this()
        {
            renderData = new RenderData(renderSettings, new DocumentToRender(document, elements));
        }

        #region IDocumentRenderer
        public async Task<byte[]> Render()
        {
            return await Task.Run(() =>
            {
                var ms = new MemoryStream();
                using (WordDocument = WordprocessingDocument.Create(ms, WordprocessingDocumentType.Document))
                {
                    WordDocument.AddMainDocumentPart();
                    WordDocument.MainDocumentPart.Document = new WordDocument
                    {
                        Body = new Body()
                    };

                    SetDocumentCommands();

                    CommandsContainer.Render();
                }
                
                return ms.ToArray();
            });
        }
        #endregion

        private void SetFooterAndHeaderCommands()
        {
            CommandsContainer.Refresh();
        }

        private void SetDocumentCommands()
        {
            CommandsContainer.Refresh();

            CommandsContainer.Add(new TableOfContentsPageCommand(WordDocument, renderData));
            CommandsContainer.Add(new ItemsCommand(WordDocument, renderData));
        }
    }
}
