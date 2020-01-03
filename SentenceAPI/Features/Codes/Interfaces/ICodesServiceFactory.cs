using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccessLayer.DatabasesManager.Interfaces;
using SharedLibrary.FactoriesManager.Interfaces;
using SharedLibrary.KernelInterfaces;

namespace SentenceAPI.Features.Codes.Interfaces
{
    interface ICodesServiceFactory : IServiceFactory
    {
        ICodesService GetService(IFactoriesManager factoriesManager, IDatabaseManager databasesManager);
    }
}
