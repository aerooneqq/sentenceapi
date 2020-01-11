using DataAccessLayer.DatabasesManager.Interfaces;
using SentenceAPI.Features.Workplace.DocumentsStorage.Interfaces;
using SentenceAPI.Features.Workplace.DocumentsStorage.Services;
using SharedLibrary.FactoriesManager.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentenceAPI.Features.Workplace.DocumentsStorage.Factories
{
    class FolderSystemServiceFactory : IFolderSystemServiceFactory
    {
        public IFileService GetFileSystem(IFactoriesManager factoriesManager, IDatabaseManager databasesManager)
        {
            return new FileService(factoriesManager, databasesManager);
        }

        public IFolderService GetFolderService(IFactoriesManager factoriesManager, IDatabaseManager databasesManager)
        {
            return new FolderService(factoriesManager, databasesManager);
        }

        public IUserMainFoldersService GetMainFolderService(IFactoriesManager factoriesManager, IDatabaseManager databaseManager)
        {
            return new UserMainFoldersService(factoriesManager, databaseManager);
        }
    }
}
