using DataAccessLayer.DatabasesManager.Interfaces;
using Domain.KernelInterfaces;
using SharedLibrary.FactoriesManager.Interfaces;


namespace Application.Workplace.DocumentStorage.FileService.Interfaces
{
    public interface IFileServiceFactory : IServiceFactory
    {
        IFileService GetService(IFactoriesManager factoriesManager, IDatabaseManager databaseManager);
    }
}