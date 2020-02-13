using DataAccessLayer.DatabasesManager.Interfaces;

using Domain.KernelInterfaces;

using SharedLibrary.FactoriesManager.Interfaces;


namespace Application.UserFriends.Interfaces
{
    public interface IUserFriendsServiceFactory : IServiceFactory
    {
        IUserFriendsService GetSerivce(IFactoriesManager factoriesManager, IDatabaseManager databasesManager);
    }
}