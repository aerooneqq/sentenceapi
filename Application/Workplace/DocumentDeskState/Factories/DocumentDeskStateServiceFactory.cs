using Application.Workplace.DocumentDeskState;

using DataAccessLayer.DatabasesManager.Interfaces;

using SentenceAPI.Features.Workplace.DocumentsDeskState.Interfaces;

using SharedLibrary.FactoriesManager.Interfaces;


namespace Application.Workplace.DocumentsDeskState.Factories
{
    public class DocumentDeskStateServiceFactory : IDocumentDeskStateServiceFactory
    {
        public IDocumentDeskStateService GetService(IFactoriesManager factoriesManager, IDatabaseManager databaseManager)
        {
            return new DocumentDeskStateService(factoriesManager, databaseManager);
        }
    }
}
