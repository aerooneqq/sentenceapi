using DataAccessLayer.DatabasesManager.Interfaces;

using Domain.KernelInterfaces;

using SharedLibrary.FactoriesManager.Interfaces;


namespace SentenceAPI.Features.UserFriends.Interfaces
{
    public interface IUserFriendsServiceFactory : IServiceFactory
    {
        IUserFriendsService GetSerivce(IFactoriesManager factoriesManager, IDatabaseManager databasesManager);
    }
}