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
        private readonly LogConfiguration logConfiguration;


        public ResponseLoggerMiddleware(RequestDelegate nextMiddlewareDel, IFactoriesManager factoriesManager)
        {
            this.nextMiddlewareDel = nextMiddlewareDel;

            factoriesManager.GetService<ILogger<ResponseLog>>().TryGetTarget(out responseLogger);

            logConfiguration = new LogConfiguration(typeof(RequestLoggerMiddleware));
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {   
            await nextMiddlewareDel.Invoke(httpContext).ConfigureAwait(false);
            
            responseLogger.Log(new ResponseLog(httpContext.Response), LogLevel.Information, logConfiguration);
        }
    }
}