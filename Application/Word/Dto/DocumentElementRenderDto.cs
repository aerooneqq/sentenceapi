using System.Collections.Generic;
using Domain.DocumentElements;
using Domain.DocumentStructureModels;

namespace Application.Word.Dto
{
    public class DocumentElementRenderDto
    {
        public List<DocumentElement> Elements { get; }
        public string Name { get; }
        public List<DocumentElementRenderDto> InnerElements { get; }

        
        public DocumentElementRenderDto(Item item)
        {
            Name = item.Name;
            Elements = new List<DocumentElement>();
            InnerElements = new List<DocumentElementRenderDto>();
        }
    }
}