using DataAccessLayer.DatabasesManager.Interfaces;
using SharedLibrary.FactoriesManager.Interfaces;

namespace Application.Templates
{
    public interface ITemplateServiceFactory
    {
        ITemplateService GetService(IDatabaseManager databaseManager, IFactoriesManager factoriesManager);
    }
}