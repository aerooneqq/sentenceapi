using Application.UserFriends.Interfaces;
using Application.UserFriends.Services;

using DataAccessLayer.DatabasesManager.Interfaces;

using SharedLibrary.FactoriesManager.Interfaces;


namespace Application.UserFriends.Factories
{
    public class UserFriendsServiceFactory : IUserFriendsServiceFactory
    {
        public IUserFriendsService GetSerivce(IFactoriesManager factoriesManager, IDatabaseManager databasesManager)
        {
            return new UserFriendsService(factoriesManager, databasesManager);
        }
    }
}
