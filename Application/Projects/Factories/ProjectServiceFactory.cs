using DataAccessLayer.DatabasesManager.Interfaces;
using SharedLibrary.FactoriesManager.Interfaces;

namespace Application.Projects
{
    public class ProjectServiceFactory : IProjectServiceFactory
    {
        public IProjectService GetService(IFactoriesManager factoriesManager, IDatabaseManager databaseManager)
        {
            return new ProjectService(factoriesManager, databaseManager);
        }
    }
}