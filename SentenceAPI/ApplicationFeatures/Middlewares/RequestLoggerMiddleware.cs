using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

using SharedLibrary.FactoriesManager.Interfaces;
using SharedLibrary.FactoriesManager;

using SharedLibrary.Loggers.Interfaces;
using SharedLibrary.Loggers.Models;
using SharedLibrary.Loggers.Configuration;


namespace SentenceAPI.ApplicationFeatures.Middlewares
{
    public class RequestLoggerMiddleware
    {
        private readonly RequestDelegate nextMiddlewareDel;
        private readonly ILogger<RequestLog> requestLogger;
        private readonly LogConfiguration logConfiguration;


        public RequestLoggerMiddleware(RequestDelegate nextMiddlewareDel, IFactoriesManager factoriesManager)
        {
            this.nextMiddlewareDel = nextMiddlewareDel;
            
            factoriesManager.GetService<ILogger<RequestLog>>().TryGetTarget(out requestLogger);

            logConfiguration = new LogConfiguration(typeof(RequestLoggerMiddleware));
        }

        /// <summary>
        /// Tries to log the request in the mongo database. If any exception occurs then this method logs
        /// the request in a request_log.txt. After all these processes the next middleware component is called.
        /// </summary>
        public async Task InvokeAsync(HttpContext httpContext)
        {
            requestLogger.Log(new RequestLog(httpContext.Request), LogLevel.Information, logConfiguration);
            //DefaultLogger.Log(new RequestLog(httpContext.Request), LogLevel.Error);

            await nextMiddlewareDel.Invoke(httpContext).ConfigureAwait(false);
        }
    }
}