using DocumentsAPI.Models;

namespace DocumentsAPI.Features.DocumentStructure.Interfaces
{
    public interface IDocumentStructureService
    {
        public Models.DocumentStructure.DocumentStructure GetDocumentStructure(long documentID);
    }
}