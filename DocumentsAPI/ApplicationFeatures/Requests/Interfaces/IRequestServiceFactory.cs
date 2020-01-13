using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DataAccessLayer.DatabasesManager.Interfaces;

using SharedLibrary.FactoriesManager.Interfaces;
using SharedLibrary.KernelInterfaces;


namespace DocumentsAPI.ApplicationFeatures.Requests.Interfaces
{
    interface IRequestServiceFactory : IServiceFactory
    {
        IRequestService GetService(IFactoriesManager factoriesManager, IDatabaseManager databaseManager);
    }
}
