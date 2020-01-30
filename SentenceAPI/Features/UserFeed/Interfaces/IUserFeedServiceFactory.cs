using DataAccessLayer.DatabasesManager.Interfaces;

using Domain.KernelInterfaces;

using SharedLibrary.FactoriesManager.Interfaces;


namespace SentenceAPI.Features.UserFeed.Interfaces
{
    public interface IUserFeedServiceFactory : IServiceFactory
    {
        IUserFeedService GetService(IFactoriesManager factoriesManager, IDatabaseManager databasesManager);
    }
}
