using DataAccessLayer.DatabasesManager.Interfaces;

using Domain.KernelInterfaces;

using SharedLibrary.FactoriesManager.Interfaces;


namespace SentenceAPI.Features.Codes.Interfaces
{
    interface ICodesServiceFactory : IServiceFactory
    {
        ICodesService GetService(IFactoriesManager factoriesManager, IDatabaseManager databasesManager);
    }
}
