using Application.Tokens.Interfaces;

using DataAccessLayer.DatabasesManager.Interfaces;

using SharedLibrary.FactoriesManager.Interfaces;


namespace Application.Tokens.Factories
{
    public class TokenServiceFactory : ITokenServiceFactory
    {
        public ITokenService GetService(IDatabaseManager databaseManager, IFactoriesManager factoriesManager)
        {
            return new TokenService(databaseManager, factoriesManager);
        }
    }
}
