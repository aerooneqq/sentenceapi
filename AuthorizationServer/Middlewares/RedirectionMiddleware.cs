using System;
using System.IO;
using System.Threading.Tasks;
using Application.Requests.Exceptions;
using Application.Requests.Interfaces;
using AuthorizationServer.Models;
using Domain.Logs;
using Domain.Logs.Configuration;
using Microsoft.AspNetCore.Http;
using SharedLibrary.FactoriesManager.Interfaces;
using SharedLibrary.Loggers.Interfaces;

using static AuthorizationServer.Startup;

namespace AuthorizationServer.Middlewares
{
    public class RedirectionMiddleware
    {
        private readonly IRequestService requestService;
        private readonly ILogger<ApplicationError> exceptionLogger;
        
        private readonly RequestDelegate nextMiddleware;
        private readonly LogConfiguration logConfiguration;

        public RedirectionMiddleware(RequestDelegate nextMiddleware, IFactoriesManager factoriesManager)
        {
            this.nextMiddleware = nextMiddleware;

            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);
            factoriesManager.GetService<IRequestService>().TryGetTarget(out requestService);
            
            logConfiguration = new LogConfiguration(GetType());
        }


        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                string url = context.Request.Path.ToUriComponent();
                string destinationUrl = GetDestinationUrlFrom(url);

                if (destinationUrl is null)
                {
                    await nextMiddleware.Invoke(context);   
                }
                else
                {
                    await requestService.RedirectRequest(context, destinationUrl);
                }
            }
            catch (RequestException ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                exceptionLogger.Log(new ApplicationError(ex.InnerException), LogLevel.Error, logConfiguration);
                context.Response.StatusCode = 500;
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                context.Response.StatusCode = 500;
            }
        }

        private static string GetDestinationUrlFrom(string url)
        {
            url = url.Substring(1);
            if (url.ToLower().Contains("sentenceapi"))
            {
                return Servers[Server.SentenceAPI] + url.Substring(url.IndexOf("/", StringComparison.Ordinal));   
            }
            
            if (url.ToLower().Contains("documentsapi"))
            {
                return Servers[Server.SentenceAPI] + url.Substring(url.IndexOf("/", StringComparison.Ordinal));
            }

            return null;
        }
    }
}