using DataAccessLayer.DatabasesManager.Interfaces;

using Domain.KernelInterfaces;

using SharedLibrary.FactoriesManager.Interfaces;


namespace DocumentsAPI.Features.Documents.Interfaces
{
    public interface IDocumentServiceFactory : IServiceFactory
    {
        IDocumentService GetService(IFactoriesManager factoriesManager, IDatabaseManager databaseManager);
    }
}