using System;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;

using SharedLibrary.FactoriesManager.Interfaces;
using SharedLibrary.FactoriesManager;

using SentenceAPI.Features.Email.Interfaces;
using SharedLibrary.Loggers.Models;
using SharedLibrary.Loggers.Interfaces;

using DataAccessLayer.CommonInterfaces;
using DataAccessLayer.Configuration.Interfaces;
using DataAccessLayer.DatabasesManager;
using DataAccessLayer.Configuration;
using DataAccessLayer.Exceptions;
using SharedLibrary.Loggers.Configuration;
using DataAccessLayer.DatabasesManager.Interfaces;

namespace SentenceAPI.Features.Email.Services
{
    public class EmailService : IEmailService
    {
        #region Static properties
        private static readonly string databaseConfigFile = "./configs/mongo_database_config.json";
        #endregion

        #region Constants
        private const string Host = "smtp.gmail.com";
        private const string ServiceMailPass = "AeroOne1";
        private const string ServerMail = "aeroone90@gmail.com";
        #endregion

        #region Databases
        private IDatabaseService<EmailLog> database;
        private IConfigurationBuilder configurationBuilder;
        #endregion

        #region Services
        private ILogger<EmailLog> emailLogger;
        private ILogger<ApplicationError> exceptionLogger;
        #endregion

        private readonly LogConfiguration logConfiguration;


        public EmailService(IFactoriesManager factoriesManager, IDatabaseManager databaseManager)
        {
            databaseManager.MongoDBFactory.GetDatabase<EmailLog>().TryGetTarget(out database);

            configurationBuilder = new MongoConfigurationBuilder(database.Configuration);
            configurationBuilder.SetConfigurationFilePath(databaseConfigFile).SetAuthMechanism()
                                .SetUserName().SetPassword().SetDatabaseName().SetServerName().SetConnectionString();

            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);
            factoriesManager.GetService<ILogger<EmailLog>>().TryGetTarget(out emailLogger);

            logConfiguration = new LogConfiguration(this.GetType());
        }

        public async Task SendConfirmationEmailAsync(string code, string email)
        {
            try
            {
                MailAddress senderAddress = new MailAddress(ServerMail);
                MailAddress userAddress = new MailAddress(email);

                using (MailMessage mailMessage = new MailMessage(senderAddress, userAddress)
                {
                    Subject = "Activation code for Sentence",
                    IsBodyHtml = true,
                    Body = $"Your code is: \n {code} \n\n Best wishes, \n Sentence team"
                })
                {
                    using (SmtpClient smtpClient = new SmtpClient(Host, 587)
                    {
                        Credentials = new NetworkCredential(ServerMail, ServiceMailPass),
                        EnableSsl = true,
                    })
                    {
                        await smtpClient.SendMailAsync(mailMessage).ConfigureAwait(false);
                        emailLogger.Log(new EmailLog(email, mailMessage.Body), LogLevel.Information, logConfiguration);
                    }
                }
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("Error occured while working with the database");
            }
        }
    }
}
