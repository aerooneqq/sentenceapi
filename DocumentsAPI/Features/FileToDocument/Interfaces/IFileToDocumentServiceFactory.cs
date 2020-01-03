using DataAccessLayer.DatabasesManager.Interfaces;

using SharedLibrary.FactoriesManager.Interfaces;


namespace DocumentsAPI.Features.FileToDocument.Interfaces
{
    public interface IFileToDocumentServiceFactory
    {
        IFileToDocumentService GetService(IFactoriesManager factoriesManager, 
                                          IDatabaseManager databaseManager);
    }
}