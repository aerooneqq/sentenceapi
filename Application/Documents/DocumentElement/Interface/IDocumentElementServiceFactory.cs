using DataAccessLayer.DatabasesManager.Interfaces;

using Domain.KernelInterfaces;

using SharedLibrary.FactoriesManager.Interfaces;


namespace Application.Documents.DocumentElement.Interface
{
    public interface IDocumentElementServiceFactory : IServiceFactory
    {
        IDocumentElementService GetService(IFactoriesManager factoriesManager, IDatabaseManager databaseManager);
    }
}