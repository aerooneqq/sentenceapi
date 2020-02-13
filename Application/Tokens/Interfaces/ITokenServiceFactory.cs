using DataAccessLayer.DatabasesManager.Interfaces;

using Domain.KernelInterfaces;
using SharedLibrary.FactoriesManager.Interfaces;


namespace Application.Tokens.Interfaces
{
    public interface ITokenServiceFactory : IServiceFactory
    {
        ITokenService GetService(IDatabaseManager databaseManager, IFactoriesManager factoriesManager);
    }
}