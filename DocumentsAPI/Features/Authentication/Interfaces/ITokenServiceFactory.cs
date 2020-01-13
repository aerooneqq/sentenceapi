using DataAccessLayer.DatabasesManager.Interfaces;

using SharedLibrary.FactoriesManager.Interfaces;


namespace DocumentsAPI.Features.Authentication.Interfaces
{
    public interface ITokenServiceFactory
    {
        ITokenService GetService(IFactoriesManager factoriesManager, IDatabaseManager databaseManger);
    }
}