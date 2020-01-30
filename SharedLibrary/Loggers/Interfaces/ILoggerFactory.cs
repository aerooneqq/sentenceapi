using Domain.KernelInterfaces;
using Domain.Logs;


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
