using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SentenceAPI.ApplicationFeatures.Loggers.Models;
using SentenceAPI.KernelInterfaces;

namespace SentenceAPI.ApplicationFeatures.Loggers.Interfaces
{
    public interface ILoggerFactory : IServiceFactory
    {
        ILogger<ApplicationError> GetExceptionLogger();
        ILogger<EmailLog> GetEmailLogger();
    }
}
