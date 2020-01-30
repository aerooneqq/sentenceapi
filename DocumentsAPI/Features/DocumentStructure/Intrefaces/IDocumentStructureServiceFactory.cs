using DataAccessLayer.DatabasesManager.Interfaces;

using Domain.KernelInterfaces;

using SharedLibrary.FactoriesManager.Interfaces;


namespace DocumentsAPI.Features.DocumentStructure.Interfaces
{
    public interface IDocumentStructureServiceFactory : IServiceFactory
    {
        IDocumentStructureService GetService(IFactoriesManager factoriesManager, IDatabaseManager databaseManager);
    }
}