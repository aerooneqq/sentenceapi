using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccessLayer.DatabasesManager.Interfaces;
using SentenceAPI.Features.Authentication.Interfaces;
using SentenceAPI.Features.Authentication.Services;
using SharedLibrary.FactoriesManager.Interfaces;

namespace SentenceAPI.Features.Authentication.Factories
{
    public class TokenServiceFactory : ITokenServiceFactory
    {
        public ITokenService GetService(IDatabaseManager databaseManager, IFactoriesManager factoriesManager)
        {
            return new TokenService(databaseManager, factoriesManager);
        }
    }
}
