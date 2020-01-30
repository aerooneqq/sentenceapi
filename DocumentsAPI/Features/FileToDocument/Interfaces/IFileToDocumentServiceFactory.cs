using DataAccessLayer.DatabasesManager.Interfaces;

using Domain.KernelInterfaces;

using SharedLibrary.FactoriesManager.Interfaces;


namespace DocumentsAPI.Features.FileToDocument.Interfaces
{
     public interface IFileToDocumentServiceFactory : IServiceFactory
     {
         IFileToDocumentService GetService(IFactoriesManager factoriesManager, 
                                           IDatabaseManager databaseManager);
     }
 }