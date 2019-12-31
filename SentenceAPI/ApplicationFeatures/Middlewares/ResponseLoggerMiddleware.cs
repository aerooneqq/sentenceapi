using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using SentenceAPI.ApplicationFeatures.Loggers;
using SentenceAPI.ApplicationFeatures.Loggers.Configuration;
using SentenceAPI.ApplicationFeatures.Loggers.Interfaces;
using SentenceAPI.ApplicationFeatures.Loggers.Models;

using SharedLibrary.FactoriesManager;
using SharedLibrary.FactoriesManager.Interfaces;


namespace SentenceAPI.ApplicationFeatures.Middlewares
{
    public class ResponseLoggerMiddleware
    {
        private readonly RequestDelegate nextMiddlewareDel;
        private readonly ILogger<ResponseLog> responseLogger;
        private readonly IFactoriesManager factoriesManager = ManagersDictionary.Instance.GetManager(Startup.ApiName);

        public ResponseLoggerMiddleware(RequestDelegate nextMiddlewareDel)
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