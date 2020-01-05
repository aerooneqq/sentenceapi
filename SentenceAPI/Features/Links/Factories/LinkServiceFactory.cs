using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccessLayer.DatabasesManager.Interfaces;
using SentenceAPI.Features.Links.Interfaces;
using SentenceAPI.Features.Links.Services;
using SharedLibrary.FactoriesManager.Interfaces;
using SharedLibrary.KernelInterfaces;

namespace SentenceAPI.Features.Links.Factories
{
    public class LinkServiceFactory : ILinkServiceFactory
    {
        public ILinkService GetService(IFactoriesManager factoriesManager, IDatabaseManager databaseManager)
        {
            return new LinkService(factoriesManager, databaseManager);
        }
    }
}
