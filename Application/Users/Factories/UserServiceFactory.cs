using Application.Users.Interfaces;
using Application.Users.Services;

using SharedLibrary.FactoriesManager.Interfaces;

using DataAccessLayer.DatabasesManager.Interfaces;

using Domain.Users;


namespace Application.Users.Factories
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
