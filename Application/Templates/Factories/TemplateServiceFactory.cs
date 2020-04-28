using Application.Templates.Interfaces;
using DataAccessLayer.DatabasesManager.Interfaces;
using SharedLibrary.FactoriesManager.Interfaces;

namespace Application.Templates.Factories
{
    public class TemplateServiceFactory : ITemplateServiceFactory
    {
        public ITemplateService GetService(IDatabaseManager databaseManager, IFactoriesManager factoriesManager)
        {
            return new TemplatesService(databaseManager, factoriesManager);
        }
    }
}