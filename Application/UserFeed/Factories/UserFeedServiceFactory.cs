using Application.UserFeed.Interfaces;

using DataAccessLayer.DatabasesManager.Interfaces;

using SharedLibrary.FactoriesManager.Interfaces;


namespace Application.UserFeed.Factories
{
    public class UserFeedServiceFactory : IUserFeedServiceFactory
    {
        public IUserFeedService GetService(IFactoriesManager factoriesManager, IDatabaseManager databasesManager)
        {
            return new UserFeedService(factoriesManager, databasesManager);
        }
    }
}
