using DataAccessLayer.DatabasesManager.Interfaces;
using Domain.KernelInterfaces;
using SharedLibrary.FactoriesManager.Interfaces;


namespace Application.Projects
{
    public interface IProjectServiceFactory : IServiceFactory
    {
        IProjectService GetService(IFactoriesManager factoriesManager, IDatabaseManager databaseManager);
    }
}