using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;

using SentenceAPI.Features.Email.Interfaces;
using SentenceAPI.Databases.MongoDB.Interfaces;
using SentenceAPI.Features.Loggers.Models;
using SentenceAPI.Features.Loggers.Interfaces;
using SentenceAPI.Features.Users.Models;
using SentenceAPI.Databases.Exceptions;
using SentenceAPI.FactoriesManager;
using SentenceAPI.Databases.CommonInterfaces;

namespace SentenceAPI.Features.Email.Services
{
    public class EmailService : IEmailService
    {
        public static LogConfiguration LogConfiguration { get; } = new LogConfiguration()
        {
            ControllerName = string.Empty,
            ServiceName = "EmailService"
        };

        #region Constants
        private const string Host = "mail.aerothedeveloper.ru";
        private const string ServiceMailPass = "QRJ-6Cf-hDV-cqG";
        private const string ServerMail = "sdwpemailservice@aerothedeveloper.ru";
        #endregion

        #region Services
        private ILogger<EmailLog> emailLogger;
        private ILogger<ApplicationError> exceptionLogger;
        private IDatabaseService<EmailLog> mongoDBService;
        #endregion

        #region Builders
        private IMongoDBServiceBuilder<EmailLog> mongoDBServiceBuilder;
        #endregion

        #region Factories
        private FactoriesManager.FactoriesManager factoriesManager = 
            FactoriesManager.FactoriesManager.Instance;
        private IMongoDBServiceFactory mongoDBServiceFactory;
        private ILoggerFactory loggerFactory;
        #endregion

        public EmailService()
        {
            mongoDBServiceFactory = factoriesManager[typeof(IMongoDBServiceFactory)].Factory as
                IMongoDBServiceFactory;
            loggerFactory = factoriesManager[typeof(ILoggerFactory)].Factory as ILoggerFactory;

            mongoDBServiceBuilder = mongoDBServiceFactory.GetBuilder(mongoDBServiceFactory.GetService<EmailLog>());
            mongoDBService = mongoDBServiceBuilder.AddConfigurationFile("database_config.json").
                SetConnectionString().SetDatabaseName("SentenceDatabase").SetCollectionName().Build();

            exceptionLogger = loggerFactory.GetExceptionLogger();
            exceptionLogger.LogConfiguration = LogConfiguration;

            emailLogger = loggerFactory.GetEmailLogger();
            emailLogger.LogConfiguration = LogConfiguration; 
        }

        public async Task SendConfirmationEmail(string link, UserInfo user)
        {
            try
            {
                MailAddress senderAddress = new MailAddress(ServerMail);
                MailAddress userAddress = new MailAddress(user.Email);

                MailMessage mailMessage = new MailMessage()
                {
                    Subject = "Activation link for Sentence",
                    IsBodyHtml = true,
                    Body = $"Your code is: \n {link} \n\n Best wishes, \n Sentence team"
                };

                SmtpClient smtpClient = new SmtpClient()
                {
                    Credentials = new NetworkCredential(ServerMail, ServiceMailPass),
                    EnableSsl = false
                };

                await Task.Run(() => smtpClient.Send(mailMessage));
                await emailLogger.Log(new EmailLog(user, mailMessage.Body));
            }
            catch (Exception ex)
            {
                await exceptionLogger.Log(new ApplicationError(ex.Message));
                throw new DatabaseException("Error occured while working with the database");
            }
        }
    }
}
