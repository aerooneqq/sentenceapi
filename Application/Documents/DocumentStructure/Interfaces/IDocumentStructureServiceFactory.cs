using DataAccessLayer.DatabasesManager.Interfaces;

using Domain.KernelInterfaces;

using SharedLibrary.FactoriesManager.Interfaces;


namespace Application.Documents.DocumentStructure.Interfaces
{
    public interface IDocumentStructureServiceFactory : IServiceFactory
    {
        IDocumentStructureService GetService(IFactoriesManager factoriesManager, IDatabaseManager databaseManager);
    }
}