using DataAccessLayer.DatabasesManager.Interfaces;

using Domain.KernelInterfaces;

using SharedLibrary.FactoriesManager.Interfaces;


namespace SentenceAPI.Features.Links.Interfaces
{
    public interface ILinkServiceFactory : IServiceFactory
    {
        ILinkService GetService(IFactoriesManager factoriesManager, IDatabaseManager databaseManager);
    }
}
