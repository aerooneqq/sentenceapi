using DataAccessLayer.DatabasesManager.Interfaces;
using SentenceAPI.Features.Workplace.DocumentsStorage.Services;
using SharedLibrary.FactoriesManager.Interfaces;
using SharedLibrary.KernelInterfaces;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentenceAPI.Features.Workplace.DocumentsStorage.Interfaces
{
    interface IFolderSystemServiceFactory : IServiceFactory
    {
        IFolderService GetFolderService(IFactoriesManager factoriesManager, IDatabaseManager databasesManager);

        IFileService GetFileSystem(IFactoriesManager factoriesManager, IDatabaseManager databasesManager);
    }
}
