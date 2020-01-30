using DataAccessLayer.DatabasesManager.Interfaces;

using Domain.KernelInterfaces;

using SharedLibrary.FactoriesManager.Interfaces;


namespace DocumentsAPI.Features.Authentication.Interfaces
{
    public interface ITokenServiceFactory : IServiceFactory
    {
        ITokenService GetService(IFactoriesManager factoriesManager, IDatabaseManager databaseManger);
    }
}