using DataAccessLayer.DatabasesManager.Interfaces;

using DocumentsAPI.Features.FileToDocument.Interfaces;
using DocumentsAPI.Features.FileToDocument.Services;

using SharedLibrary.FactoriesManager.Interfaces;


namespace DocumentsAPI.Features.FileToDocument.Factories
{
    public class FileToDocumentServiceFactory : IFileToDocumentServiceFactory
    {
        public IFileToDocumentService GetService(IFactoriesManager factoriesManager, 
                                                 IDatabaseManager databaseManager)
        {
            return new FileToDocumentService(factoriesManager, databaseManager);
        }
    }
}