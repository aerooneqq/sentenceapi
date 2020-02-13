using Application.Links.Interfaces;
using Application.Links.Services;

using DataAccessLayer.DatabasesManager.Interfaces;

using SharedLibrary.FactoriesManager.Interfaces;


namespace Application.Links.Factories
{
    public class LinkServiceFactory : ILinkServiceFactory
    {
        public ILinkService GetService(IFactoriesManager factoriesManager, IDatabaseManager databaseManager)
        {
            return new LinkService(factoriesManager, databaseManager);
        }
    }
}
