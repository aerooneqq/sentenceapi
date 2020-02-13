using System.Security.Cryptography.Xml;
using Application.Workplace.DocumentStorage.FileService.Interfaces;
using DataAccessLayer.DatabasesManager.Interfaces;
using SharedLibrary.FactoriesManager.Interfaces;

namespace Application.Workplace.DocumentStorage.Services.Factories
{
    public class FileServiceFactory : IFileServiceFactory
    {
        public IFileService GetService(IFactoriesManager factoriesManager, IDatabaseManager databaseManager)
        {
            return new FileService.FileService(factoriesManager, databaseManager);
        }
    }
}