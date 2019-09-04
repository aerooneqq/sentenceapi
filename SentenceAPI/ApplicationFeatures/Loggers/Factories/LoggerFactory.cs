using SentenceAPI.ApplicationFeatures.Loggers.Interfaces;
using SentenceAPI.ApplicationFeatures.Loggers;
using SentenceAPI.ApplicationFeatures.Loggers.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentenceAPI.ApplicationFeatures.Loggers.Factories
{
    public class LoggerFactory : ILoggerFactory
    {
        public ILogger<ApplicationError> GetExceptionLogger()
        {
            return new ExceptionLogger();
        }

        public ILogger<EmailLog> GetEmailLogger()
        {
            return new EmailLogger();
        }
    }
}
