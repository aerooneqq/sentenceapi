using Application.Workplace.DocumentStorage.FolderService.Interfaces;
using DataAccessLayer.DatabasesManager.Interfaces;
using SharedLibrary.FactoriesManager.Interfaces;

namespace Application.Workplace.DocumentStorage.FolderService.Factories
{
    public class FolderServiceFactory : IFolderServiceFactory
    {
        public IFolderService GetService(IFactoriesManager factoriesManager, IDatabaseManager databaseManager)
        {
            return new FolderService(factoriesManager, databaseManager);
        }
    }
}