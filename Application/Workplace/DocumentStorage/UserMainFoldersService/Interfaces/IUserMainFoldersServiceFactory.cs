using DataAccessLayer.DatabasesManager.Interfaces;
using Domain.KernelInterfaces;
using SharedLibrary.FactoriesManager.Interfaces;

namespace Application.Workplace.DocumentStorage.UserMainFoldersService.Interfaces
{
    public interface IUserMainFoldersServiceFactory : IServiceFactory
    {
        IUserMainFoldersService GetService(IFactoriesManager factoriesManager, IDatabaseManager databaseManager);
    }
}