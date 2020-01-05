using DataAccessLayer.DatabasesManager.Interfaces;

using SharedLibrary.FactoriesManager.Interfaces;
using SharedLibrary.KernelInterfaces;


namespace DocumentsAPI.Features.DocumentStructure.Interfaces
{
    public interface IDocumentStructureServiceFactory : IServiceFactory
    {
        IDocumentStructureService GetService(IFactoriesManager factoriesManager, IDatabaseManager databaseManager);
    }
}