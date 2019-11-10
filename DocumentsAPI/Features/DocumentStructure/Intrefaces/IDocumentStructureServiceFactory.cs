using DocumentsAPI.Features.DocumentStructure.Services;

namespace DocumentsAPI.Features.DocumentStructure.Interfaces
{ 
    public interface IDocumentStructureServiceFactory
    {
        public IDocumentStructureService GetService()
        { 
            return new DocumentStructureService();
        }
    }
}