using DataAccessLayer.DatabasesManager.Interfaces;

using Domain.KernelInterfaces;

using SharedLibrary.FactoriesManager.Interfaces;


namespace Application.Codes.Interfaces
{
    public interface ICodesServiceFactory : IServiceFactory
    {
        ICodesService GetService(IFactoriesManager factoriesManager, IDatabaseManager databasesManager);
    }
}
