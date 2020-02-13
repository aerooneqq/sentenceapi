using DataAccessLayer.DatabasesManager.Interfaces;

using Domain.KernelInterfaces;

using SharedLibrary.FactoriesManager.Interfaces;


namespace Application.Links.Interfaces
{
    public interface ILinkServiceFactory : IServiceFactory
    {
        ILinkService GetService(IFactoriesManager factoriesManager, IDatabaseManager databaseManager);
    }
}
