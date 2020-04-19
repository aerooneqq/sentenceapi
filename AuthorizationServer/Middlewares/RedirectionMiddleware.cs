using System;
using System.IO;
using System.Net;
using System.Text;
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
                string url = context.Request.Path.Value;
                string queryParams = context.Request.QueryString.Value;
                string destinationUrl = GetDestinationUrlFrom(url, queryParams);

                if (context.Request.Method.ToLower() == "options") 
                {
                    context.Response.OnStarting((state) => 
                    { 
                        var response = (HttpResponse)state;

                        response.Headers.Add("Access-Control-Allow-Origin", "*");
                        response.Headers.Add("Access-Control-Allow-Methods", "*");
                        response.Headers.Add("Access-Control-Allow-Headers", "*");

                        return Task.CompletedTask;
                    }, context.Response);
                    
                    return;
                }
                else if (destinationUrl is null)
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

                WebException innerException = (WebException)ex.InnerException;
                context.Response.StatusCode = GetStatusCodeFromWebException(innerException) ?? 500;
                await context.Response.WriteAsync(await GetWebExceptionResponseMessage(innerException), Encoding.UTF8);
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
            }
        }

        private async Task<string> GetWebExceptionResponseMessage(WebException ex)
        {
            StreamReader sr = new StreamReader(ex.Response.GetResponseStream());

            return await sr.ReadToEndAsync().ConfigureAwait(false);
        }

        private int? GetStatusCodeFromWebException(WebException ex)
        {
            if (ex.Status == WebExceptionStatus.ProtocolError)
            {
                var response = ex.Response as HttpWebResponse;
                if (response != null)
                {
                    return (int)response.StatusCode;
                }
            }

            return null;
        }

        private static string GetDestinationUrlFrom(string url, string queryParams)
        {
            url = url.Substring(1);
            if (url.ToLower().Contains("sentenceapi"))
            {
                return Servers[Server.SentenceAPI] + url.Substring(url.IndexOf("/", StringComparison.Ordinal)) + queryParams;   
            }
            
            if (url.ToLower().Contains("documentsapi"))
            {
                return Servers[Server.DocumentsAPI] + url.Substring(url.IndexOf("/", StringComparison.Ordinal)) + queryParams;
            }

            return null;
        }
    }
}