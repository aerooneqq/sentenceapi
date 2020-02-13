using DataAccessLayer.DatabasesManager.Interfaces;
using Domain.KernelInterfaces;
using SharedLibrary.FactoriesManager.Interfaces;

namespace SentenceAPI.Features.Workplace.DocumentsDeskState.Interfaces
{
    public interface IDocumentDeskStateServiceFactory : IServiceFactory
    {
        IDocumentDeskStateService GetService(IFactoriesManager factoriesManager, IDatabaseManager databaseManager);
    }
}
