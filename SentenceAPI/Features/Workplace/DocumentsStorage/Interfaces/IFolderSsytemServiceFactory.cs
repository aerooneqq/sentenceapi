using DataAccessLayer.DatabasesManager.Interfaces;

using SharedLibrary.FactoriesManager.Interfaces;

using Domain.KernelInterfaces;


namespace SentenceAPI.Features.Workplace.DocumentsStorage.Interfaces
{
    internal interface IFolderSystemServiceFactory : IServiceFactory
    {
        IFolderService GetFolderService(IFactoriesManager factoriesManager, IDatabaseManager databasesManager);

        IFileService GetFileSystem(IFactoriesManager factoriesManager, IDatabaseManager databasesManager);
    }
}
