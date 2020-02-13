using DataAccessLayer.DatabasesManager.Interfaces;
using DocumentsAPI.Features.Documents.Interfaces;
using DocumentsAPI.Features.Documents.Services;
using SharedLibrary.FactoriesManager.Interfaces;

namespace DocumentsAPI.Features.Documents.Factories
{
    public class DocumentServiceFactory : IDocumentServiceFactory
    {
        public IDocumentService GetService(IFactoriesManager factoriesManager, IDatabaseManager databaseManager)
        {
            return new DocumentService(factoriesManager, databaseManager);
        }
    }
}