using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

using SharedLibrary.FactoriesManager.Interfaces;
using SharedLibrary.FactoriesManager;

using SentenceAPI.ApplicationFeatures.Loggers.Interfaces;
using SentenceAPI.ApplicationFeatures.Loggers.Models;
using SentenceAPI.ApplicationFeatures.Loggers.Configuration;


namespace SentenceAPI.ApplicationFeatures.Middlewares
{
    public class RequestLoggerMiddleware
    {
        private readonly RequestDelegate nextMiddlewareDel;
        private readonly ILogger<RequestLog> requestLogger;
        private readonly IFactoriesManager factoriesManager = ManagersDictionary.Instance.GetManager(Startup.ApiName);

        public RequestLoggerMiddleware(RequestDelegate nextMiddlewareDel)
        {
            this.nextMiddlewareDel = nextMiddlewareDel;
            
            factoriesManager.GetService<ILogger<RequestLog>>().TryGetTarget(out requestLogger);

            requestLogger.LogConfiguration = new LogConfiguration(typeof(RequestLoggerMiddleware))
            {
                ClassName = this.GetType().FullName,
                ComponentType = ComponentType.Middleware
            };
        }

        /// <summary>
        /// Tries to log the request in the mongo database. If any exception occurs then this method logs
        /// the request in a request_log.txt. After all these processes the next middleware component is called.
        /// </summary>
        public async Task InvokeAsync(HttpContext httpContext)
        {
            requestLogger.Log(new RequestLog(httpContext.Request), LogLevel.Information);

            await nextMiddlewareDel.Invoke(httpContext).ConfigureAwait(false);
        }
    }
}