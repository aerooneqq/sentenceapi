using Application.Workplace.DocumentStorage.UserMainFoldersService.Interfaces;
using DataAccessLayer.DatabasesManager.Interfaces;
using SharedLibrary.FactoriesManager.Interfaces;

namespace Application.Workplace.DocumentStorage.UserMainFoldersService.Factories
{
    public class UserMainFoldersServiceFactory : IUserMainFoldersServiceFactory
    {
        public IUserMainFoldersService GetService(IFactoriesManager factoriesManager, IDatabaseManager databaseManager)
        {
            return new UserMainFoldersService(factoriesManager, databaseManager);
        }
    }
}