using DataAccessLayer.DatabasesManager.Interfaces;

using SharedLibrary.FactoriesManager.Interfaces;
using SharedLibrary.KernelInterfaces;


namespace DocumentsAPI.Features.FileToDocument.Interfaces
{
    public interface IFileToDocumentServiceFactory : IServiceFactory
    {
        IFileToDocumentService GetService(IFactoriesManager factoriesManager, 
                                          IDatabaseManager databaseManager);
    }
}