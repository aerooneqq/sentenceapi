using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;

using SentenceAPI.Features.Email.Interfaces;
using SentenceAPI.Features.Loggers.Models;
using SentenceAPI.Features.Loggers.Interfaces;
using SentenceAPI.Features.Users.Models;
using DataAccessLayer.CommonInterfaces;
using DataAccessLayer.Configuration.Interfaces;
using DataAccessLayer.DatabasesManager;
using DataAccessLayer.Configuration;
using DataAccessLayer.Exceptions;

namespace SentenceAPI.Features.Email.Services
{
    public class EmailService : IEmailService
    {
        #region Static properties
        private static readonly string databaseConfigFile = "mongo_database_config.json";
        private static LogConfiguration LogConfiguration { get; } = new LogConfiguration()
        {
            ControllerName = string.Empty,
            ServiceName = "EmailService"
        };
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

        #region Factories
        private FactoriesManager.FactoriesManager factoriesManager = 
            FactoriesManager.FactoriesManager.Instance;
        #endregion

        public EmailService()
        {
            DatabasesManager.Manager.MongoDBFactory.GetDatabase<EmailLog>().TryGetTarget(out database);

            configurationBuilder = new MongoConfigurationBuilder(database.Configuration);
            configurationBuilder.SetConfigurationFilePath(databaseConfigFile).SetAuthMechanism()
                                .SetUserName().SetPassword().SetDatabaseName().SetServerName().SetConnectionString();

            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);
            factoriesManager.GetService<ILogger<EmailLog>>().TryGetTarget(out emailLogger);

            exceptionLogger.LogConfiguration = LogConfiguration;
            emailLogger.LogConfiguration = LogConfiguration; 
        }

        public async Task SendConfirmationEmail(string code, string email)
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
                        EnableSsl = true
                    })
                    {
                        await smtpClient.SendMailAsync(mailMessage);
                        await emailLogger.Log(new EmailLog(email, mailMessage.Body));
                    }
                }
            }
            catch (Exception ex)
            {
                await exceptionLogger.Log(new ApplicationError(ex.Message));
                throw new DatabaseException("Error occured while working with the database");
            }
        }
    }
}
