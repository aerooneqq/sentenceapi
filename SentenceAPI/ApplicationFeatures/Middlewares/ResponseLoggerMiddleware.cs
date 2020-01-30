using System.Threading.Tasks;

using Domain.Logs;
using Domain.Logs.Configuration;
using Microsoft.AspNetCore.Http;

using SharedLibrary.FactoriesManager.Interfaces;
using SharedLibrary.Loggers.Interfaces;


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