using Application.Email.Interfaces;
using Application.Email.Services;

using DataAccessLayer.DatabasesManager.Interfaces;

using SharedLibrary.FactoriesManager.Interfaces;


namespace Application.Email.Factories
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
