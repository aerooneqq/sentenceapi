using Application.Documents.DocumentElement.Interface;

using DataAccessLayer.DatabasesManager.Interfaces;

using SharedLibrary.FactoriesManager.Interfaces;


namespace Application.Documents.DocumentElement.Factories
{
    public class DocumentElementServiceFactory : IDocumentElementServiceFactory
    {
        public IBranchService GetBranchService(IFactoriesManager factoriesManager, IDatabaseManager databaseManager)
        {
            return new BranchService(factoriesManager, databaseManager);
        }

        public IDocumentElementService GetElementService(IFactoriesManager factoriesManager, IDatabaseManager databaseManager)
        {
            return new DocumentElementService(factoriesManager, databaseManager);
        }

        public IBranchNodeService GetNodeService(IFactoriesManager factoriesManager, IDatabaseManager databaseManager)
        {
            return new BranchNodeService(factoriesManager, databaseManager);
        }
    }
}