using DataAccessLayer.DatabasesManager.Interfaces;
using Domain.KernelInterfaces;
using SharedLibrary.FactoriesManager.Interfaces;

namespace Application.Templates.Interfaces
{
    public interface ITemplateServiceFactory : IServiceFactory
    {
        ITemplateService GetService(IDatabaseManager databaseManager, IFactoriesManager factoriesManager);
    }
}