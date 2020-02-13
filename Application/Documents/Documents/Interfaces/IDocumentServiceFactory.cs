using DataAccessLayer.DatabasesManager.Interfaces;

using Domain.KernelInterfaces;

using SharedLibrary.FactoriesManager.Interfaces;


namespace Application.Documents.Documents.Interfaces
{
    public interface IDocumentServiceFactory : IServiceFactory
    {
        IDocumentService GetService(IFactoriesManager factoriesManager, IDatabaseManager databaseManager);
    }
}