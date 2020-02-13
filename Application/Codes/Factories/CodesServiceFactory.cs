using DataAccessLayer.DatabasesManager.Interfaces;

using SharedLibrary.FactoriesManager.Interfaces;

using Application.Codes.Interfaces;
using Application.Codes.Services;


namespace Application.Codes.Factories
{
    public class CodesServiceFactory : ICodesServiceFactory
    {
        public ICodesService GetService(IFactoriesManager factoriesManager, IDatabaseManager databasesManager)
        {
            return new CodesService(factoriesManager, databasesManager);
        }
    }
}
