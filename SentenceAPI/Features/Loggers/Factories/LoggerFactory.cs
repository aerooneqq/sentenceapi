using SentenceAPI.Features.Loggers.Interfaces;
using SentenceAPI.Features.Logger;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SentenceAPI.Features.Loggers.Models;

namespace SentenceAPI.Features.Loggers.Factories
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
