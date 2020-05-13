using System.Collections.Generic;
using Domain.Models.Document;

namespace Application.Word.Dto
{
    public class DocumentToRender
    {
        public string Name { get; }
        public List<DocumentElementRenderDto> Elements { get; }
        
        public DocumentToRender(Document document, List<DocumentElementRenderDto> elements)
        {
            Name = document.Name;
            Elements = elements;
        }
    }
}