using DataAccessLayer.DatabasesManager.Interfaces;
using Domain.KernelInterfaces;
using SharedLibrary.FactoriesManager.Interfaces;

namespace Application.Workplace.DocumentStorage.FolderService.Interfaces
{
    public interface IFolderServiceFactory : IServiceFactory
    {
        IFolderService GetService(IFactoriesManager factoriesManager, IDatabaseManager databaseManager);
    }
}