﻿﻿using System;
using System.Threading.Tasks;

using Application.Codes.Interfaces;
using Application.Email.Interfaces;
  
using DataAccessLayer.Exceptions;

using Domain.Codes;
using Domain.Logs;
using Domain.Logs.Configuration;

using SharedLibrary.Loggers.Interfaces;
using SharedLibrary.FactoriesManager.Interfaces;
  
using MongoDB.Bson;

using SharedLibrary.Events.Interfaces;


namespace SentenceAPI.Features.Users.Events
{
    /// <summary>
    /// This "event" is used when the user email was changed.
    /// </summary>
    public class UserEmailChangedEvent : IDomainEvent
    {
        #region Fields
        private readonly string email;
        private readonly ObjectId userID;
        #endregion


        #region Services
        private readonly IEmailService emailService;
        private readonly ICodesService codesService;
        private readonly ILogger<ApplicationError> exceptionLogger;
        #endregion

        private readonly LogConfiguration logConfiguration;

        public UserEmailChangedEvent(string email, ObjectId userID, IFactoriesManager factoriesManager)
        {
            this.email = email;
            this.userID = userID;

            factoriesManager.GetService<IEmailService>().TryGetTarget(out emailService);
            factoriesManager.GetService<ICodesService>().TryGetTarget(out codesService);
            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);
            
            logConfiguration = new LogConfiguration(this.GetType());
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
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("The error occured while sending confirmation email");
            }
        }
        #endregion
    }
}
