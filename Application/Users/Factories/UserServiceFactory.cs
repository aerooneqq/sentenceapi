using SentenceAPI.Features.Users.Interfaces;
using SentenceAPI.Features.Users.Services;

using SharedLibrary.FactoriesManager.Interfaces;

using DataAccessLayer.DatabasesManager.Interfaces;

using Domain.Users;


namespace SentenceAPI.Features.Users.Factories
{
    public class UserServiceFactory : IUserServiceFactory
    {
        public IUserService<UserInfo> GetService(IFactoriesManager factoriesManager, 
                                                 IDatabaseManager databasesManager)
        {
            return new UserService(factoriesManager, databasesManager);
        }
    }
}
