using DataAccessLayer.DatabasesManager.Interfaces;

using Domain.KernelInterfaces;

using SharedLibrary.FactoriesManager.Interfaces;


namespace SentenceAPI.ApplicationFeatures.Requests.Interfaces
{
    interface IRequestServiceFactory : IServiceFactory
    {
        IRequestService GetService(IFactoriesManager factoriesManager, IDatabaseManager databaseManager);
    }
}
