using DataAccessLayer.DatabasesManager.Interfaces;
using Domain.KernelInterfaces;
using SharedLibrary.FactoriesManager.Interfaces;

namespace Application.Requests.Interfaces
{
    public interface IRequestServiceFactory : IServiceFactory
    {
        IRequestService GetService(IFactoriesManager factoriesManager, IDatabaseManager databaseManager);
    }
}