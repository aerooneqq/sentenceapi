using DataAccessLayer.DatabasesManager.Interfaces;

using SentenceAPI.Features.Email.Interfaces;
using SentenceAPI.Features.Email.Services;

using SharedLibrary.FactoriesManager.Interfaces;


namespace SentenceAPI.Features.Email.Factories
{
    public class EmailServiceFactory : IEmailServiceFactory
    {
        public IEmailService 
            GetService(IFactoriesManager factoriesManager, IDatabaseManager databasesManager)
        {
            return new EmailService(factoriesManager, databasesManager);
        }
    }
}
