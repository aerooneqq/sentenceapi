using DataAccessLayer.DatabasesManager.Interfaces;

using Domain.KernelInterfaces;

using SharedLibrary.FactoriesManager.Interfaces;


namespace SentenceAPI.Features.Email.Interfaces
{
    public interface IEmailServiceFactory : IServiceFactory
    {
        IEmailService GetService(IFactoriesManager factoriesManager, IDatabaseManager databasesManager);
    }
}
