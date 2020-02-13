using System;
using System.Threading.Tasks;

using Application.Tokens.Interfaces;
using Domain.Authentication;
using Domain.Logs;
using Domain.Logs.Configuration;
using Microsoft.AspNetCore.Http;

using SharedLibrary.Loggers.Interfaces;


namespace AuthorizationServer.Middlewares
{
    public class AuthorizationMiddleware
    {
        private readonly ILogger<ApplicationError> exceptionLogger;
        private readonly ITokenService tokenService;

        private readonly RequestDelegate nextMiddleware;
        private readonly LogConfiguration logConfiguration;


        public AuthorizationMiddleware(RequestDelegate nextMiddleware)
        {
            this.nextMiddleware = nextMiddleware;
        }


        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                string token = context.Request.Headers["Authorization"].ToString();

                if (!tokenService.CheckToken(token)) 
                {
                    context.Response.StatusCode = 401;
                    return;
                }

                await nextMiddleware(context);
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                context.Response.StatusCode = 401;
            }
        }
    }
}