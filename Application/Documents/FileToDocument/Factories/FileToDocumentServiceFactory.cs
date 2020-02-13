using Application.Documents.FileToDocument.Interfaces;

using DataAccessLayer.DatabasesManager.Interfaces;

using SharedLibrary.FactoriesManager.Interfaces;


namespace Application.Documents.FileToDocument.Factories
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