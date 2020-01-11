using DataAccessLayer.DatabasesManager.Interfaces;

using SharedLibrary.FactoriesManager.Interfaces;
using SharedLibrary.KernelInterfaces;


namespace DocumentsAPI.Features.Documents.Interfaces
{
    public interface IDocumentServiceFactory : IServiceFactory
    {
        IDocumentService GetService(IFactoriesManager factoriesManager, IDatabaseManager databaseManager);
    }
}