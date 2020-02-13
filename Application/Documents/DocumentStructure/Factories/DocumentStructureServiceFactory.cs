using Application.Documents.DocumentStructure.Interfaces;

using DataAccessLayer.DatabasesManager.Interfaces;

using SharedLibrary.FactoriesManager.Interfaces;


namespace Application.Documents.DocumentStructure.Factories
{
    public class DocumentStructureServiceFactory : IDocumentStructureServiceFactory
    {
        public IDocumentStructureService GetService(IFactoriesManager factoriesManager, IDatabaseManager databaseManager)
        {
            return new DocumentStructureService(factoriesManager, databaseManager);
        }
    }
}