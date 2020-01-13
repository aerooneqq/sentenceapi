using DataAccessLayer.DatabasesManager.Interfaces;

using DocumentsAPI.Features.Authentication.Interfaces;
using DocumentsAPI.Features.Authentication.Services;

using SharedLibrary.FactoriesManager.Interfaces;


namespace DocumentsAPI.Features.Authentication.Factories
{
    public class TokenServiceFactory : ITokenServiceFactory
    {
        public ITokenService GetService(IFactoriesManager factoriesManager, IDatabaseManager databaseManager)
        {
            return new TokenService(factoriesManager, databaseManager);
        }
    }
}