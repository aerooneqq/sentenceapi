using DataAccessLayer.DatabasesManager.Interfaces;
using DocumentsAPI.Features.DocumentStructure.Interfaces;
using DocumentsAPI.Features.DocumentStructure.Services;
using SharedLibrary.FactoriesManager.Interfaces;

namespace DocumentsAPI.Features.DocumentStructure.Factories
{
    public class DocumentStructureServiceFactory : IDocumentStructureServiceFactory
    {
        public IDocumentStructureService GetService(IFactoriesManager factoriesManager, IDatabaseManager databaseManager)
        {
            return new DocumentStructureService(factoriesManager, databaseManager);
        }
    }
}