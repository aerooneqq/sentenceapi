using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccessLayer.DatabasesManager.Interfaces;
using SharedLibrary.FactoriesManager.Interfaces;
using SharedLibrary.KernelInterfaces;

namespace SentenceAPI.Features.Links.Interfaces
{
    public interface ILinkServiceFactory : IServiceFactory
    {
        ILinkService GetService(IFactoriesManager factoriesManager, IDatabaseManager databaseManager);
    }
}
