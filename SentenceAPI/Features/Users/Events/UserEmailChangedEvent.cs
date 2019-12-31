using DataAccessLayer.Exceptions;

using SharedLibrary.Loggers.Interfaces;
using SharedLibrary.Loggers.Models;
using SentenceAPI.Events.Interfaces;
using SentenceAPI.Features.Codes.Interfaces;
using SentenceAPI.Features.Codes.Models;
using SentenceAPI.Features.Email.Interfaces;

using System;
using System.Threading.Tasks;

using SharedLibrary.FactoriesManager.Interfaces;
using SharedLibrary.FactoriesManager;
using SharedLibrary.Loggers.Configuration;


namespace SentenceAPI.Features.Users.Events
{
    /// <summary>
    /// This "event" is used when the user email was changed.
    /// </summary>
    public class UserEmailChangedEvent : IDomainEvent
    {
        #region Fields
        private readonly string email;
        private readonly long userID;
        #endregion

        #region Factories
        private readonly IFactoriesManager factoriesManager = 
            ManagersDictionary.Instance.GetManager(Startup.ApiName);
        #endregion

        #region Services
        private IEmailService emailService;
        private ICodesService codesService;
        private ILogger<ApplicationError> exceptionLogger;
        #endregion

        public UserEmailChangedEvent(string email, long userID)
        {
            this.email = email;
            this.userID = userID;

            factoriesManager.GetService<IEmailService>().TryGetTarget(out emailService);
            factoriesManager.GetService<ICodesService>().TryGetTarget(out codesService);
            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);
            
            exceptionLogger.LogConfiguration = new LogConfiguration(this.GetType());
        }

        #region IDomainEvent implementation 
        /// <summary>
        /// Creates an activation code and sends this code to the user on the given email.
        /// </summary>
        /// <exception cref="DatabaseException">
        /// Throws this exception if the error occurs while working with the database service.
        /// This exception provides the meaningful description for the error, so this message
        /// can be returned to the user.
        /// </exception>
        public async Task Handle()
        {
            try
            {
                ActivationCode activationCode = codesService.CreateActivationCode(userID);
                await codesService.InsertCodeInDatabaseAsync(activationCode).ConfigureAwait(false);

                await emailService.SendConfirmationEmailAsync(activationCode.Code, email).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error);
                throw new DatabaseException("The error occured while sending confirmation email");
            }
        }
        #endregion
    }
}
