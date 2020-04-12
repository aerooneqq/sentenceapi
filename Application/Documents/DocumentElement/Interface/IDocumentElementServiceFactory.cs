using DataAccessLayer.DatabasesManager.Interfaces;

using Domain.KernelInterfaces;

using SharedLibrary.FactoriesManager.Interfaces;


namespace Application.Documents.DocumentElement.Interface
{
    public interface IDocumentElementServiceFactory : IServiceFactory
    {
        IDocumentElementService GetElementService(IFactoriesManager factoriesManager, IDatabaseManager databaseManager);
        IBranchNodeService GetNodeService(IFactoriesManager factoriesManager, IDatabaseManager databaseManager);
        IBranchService GetBranchService(IFactoriesManager factoriesManager, IDatabaseManager databaseManager);
    }
}