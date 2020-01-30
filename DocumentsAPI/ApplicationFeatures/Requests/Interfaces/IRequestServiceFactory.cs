using DataAccessLayer.DatabasesManager.Interfaces;

using Domain.KernelInterfaces;

using SharedLibrary.FactoriesManager.Interfaces;


namespace DocumentsAPI.ApplicationFeatures.Requests.Interfaces
{
    internal interface IRequestServiceFactory : IServiceFactory
    {
        IRequestService GetService(IFactoriesManager factoriesManager, IDatabaseManager databaseManager);
    }
}
