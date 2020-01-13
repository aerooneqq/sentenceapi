using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DataAccessLayer.DatabasesManager.Interfaces;

using DocumentsAPI.ApplicationFeatures.Requests.Interfaces;

using SharedLibrary.FactoriesManager.Interfaces;


namespace DocumentsAPI.ApplicationFeatures.Requests.Factories
{
    class RequestServiceFactory : IRequestServiceFactory
    {
        public IRequestService GetService(IFactoriesManager factoriesManager, IDatabaseManager databaseManager)
        {
            return new RequestService(factoriesManager, databaseManager);
        }
    }
}
