using DataAccessLayer.DatabasesManager.Interfaces;

using Domain.KernelInterfaces;
using Domain.Users;

using SharedLibrary.FactoriesManager.Interfaces;


namespace Application.Users.Interfaces
{
    public interface IUserServiceFactory : IServiceFactory
    {
        IUserService<UserInfo> GetService(IFactoriesManager factoriesManager, IDatabaseManager databasesManager);
    }
}
