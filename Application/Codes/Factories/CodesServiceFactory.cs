using DataAccessLayer.DatabasesManager.Interfaces;
using SentenceAPI.Features.Codes.Interfaces;
using SentenceAPI.Features.Codes.Services;
using SharedLibrary.FactoriesManager.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentenceAPI.Features.Codes.Factories
{
    class CodesServiceFactory : ICodesServiceFactory
    {
        public ICodesService GetService(IFactoriesManager factoriesManager, IDatabaseManager databasesManager)
        {
            return new CodesService(factoriesManager, databasesManager);
        }
    }
}
