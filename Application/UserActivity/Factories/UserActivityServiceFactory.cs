using Application.UserActivity.Interfaces;

using DataAccessLayer.DatabasesManager.Interfaces;


namespace Application.UserActivity.Factories
{
    public class UserActivityServiceFactory : IUserActivityServiceFactory
    {
        public IUserActivityService GetService(IDatabaseManager databasesManager)
        {
            return new UserActivityService(databasesManager);
        }
    }
}
