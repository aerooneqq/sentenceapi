using Application.Documents.Documents.Interfaces;

using DataAccessLayer.DatabasesManager.Interfaces;

using SharedLibrary.FactoriesManager.Interfaces;


namespace Application.Documents.Documents.Factories
{
    public class DocumentServiceFactory : IDocumentServiceFactory
    {
        public IDocumentService GetService(IFactoriesManager factoriesManager, IDatabaseManager databaseManager)
        {
            return new DocumentService(factoriesManager, databaseManager);
        }
    }
}