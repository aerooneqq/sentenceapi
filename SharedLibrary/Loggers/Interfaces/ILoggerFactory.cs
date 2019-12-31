using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SharedLibrary.Loggers.Models;

using SharedLibrary.KernelInterfaces;

namespace SharedLibrary.Loggers.Interfaces
{
    public interface ILoggerFactory : IServiceFactory
    {
        ILogger<ApplicationError> GetExceptionLogger();
        ILogger<EmailLog> GetEmailLogger();
        ILogger<RequestLog> GetRequestLog();
        ILogger<ResponseLog> GetResponseLog();
    }
}
