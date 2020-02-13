using DataAccessLayer.DatabasesManager.Interfaces;

using Domain.KernelInterfaces;

using SharedLibrary.FactoriesManager.Interfaces;


namespace Application.Documents.FileToDocument.Interfaces
{
     public interface IFileToDocumentServiceFactory : IServiceFactory
     {
         IFileToDocumentService GetService(IFactoriesManager factoriesManager, 
                                           IDatabaseManager databaseManager);
     }
 }