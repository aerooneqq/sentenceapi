using SentenceAPI.ApplicationFeatures.Requests.Interfaces;
using DataAccessLayer.DatabasesManager.Interfaces;
using SharedLibrary.FactoriesManager.Interfaces;

namespace SentenceAPI.ApplicationFeatures.Requests.Factories
{
    internal class RequestServiceFactory : IRequestServiceFactory
    {
        public IRequestService GetService(IFactoriesManager factoriesManager, IDatabaseManager databaseManager)
        {
            return new RequestService(factoriesManager, databaseManager);
        }
    }
}
