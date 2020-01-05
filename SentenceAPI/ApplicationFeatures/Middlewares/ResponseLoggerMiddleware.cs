using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

using SharedLibrary.FactoriesManager;
using SharedLibrary.FactoriesManager.Interfaces;
using SharedLibrary.Loggers.Configuration;
using SharedLibrary.Loggers.Interfaces;
using SharedLibrary.Loggers.Models;

namespace SentenceAPI.ApplicationFeatures.Middlewares
{
    public class ResponseLoggerMiddleware
    {
        private readonly RequestDelegate nextMiddlewareDel;
        private readonly ILogger<ResponseLog> responseLogger;


        public ResponseLoggerMiddleware(RequestDelegate nextMiddlewareDel, IFactoriesManager factoriesManager)
        {
            this.nextMiddlewareDel = nextMiddlewareDel;

            factoriesManager.GetService<ILogger<ResponseLog>>().TryGetTarget(out responseLogger);

            responseLogger.LogConfiguration = new LogConfiguration(typeof(RequestLoggerMiddleware))
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
            await nextMiddlewareDel.Invoke(httpContext).ConfigureAwait(false);
            
            //DefaultLogger.Log(new ResponseLog(httpContext.Response), LogLevel.Information);
            responseLogger.Log(new ResponseLog(httpContext.Response), LogLevel.Information);
        }
    }
}