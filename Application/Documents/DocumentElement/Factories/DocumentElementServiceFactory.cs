using Application.Documents.DocumentElement.Interface;

using DataAccessLayer.DatabasesManager.Interfaces;

using SharedLibrary.FactoriesManager.Interfaces;


namespace Application.Documents.DocumentElement.Factories
{
    public class DocumentElementServiceFactory : IDocumentElementServiceFactory
    {
        public IDocumentElementService GetService(IFactoriesManager factoriesManager, IDatabaseManager databaseManager)
        {
            return new DocumentElementService(factoriesManager, databaseManager);
        }
    }
}