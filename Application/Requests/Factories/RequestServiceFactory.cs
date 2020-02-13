using Application.Requests.Interfaces;
using DataAccessLayer.DatabasesManager.Interfaces;
using SharedLibrary.FactoriesManager.Interfaces;

namespace Application.Requests.Factories
{
    public class RequestServiceFactory : IRequestServiceFactory
    {
        public IRequestService GetService(IFactoriesManager factoriesManager, IDatabaseManager databaseManager)
        {
            return new RequestService(factoriesManager, databaseManager);
        }
    }
}