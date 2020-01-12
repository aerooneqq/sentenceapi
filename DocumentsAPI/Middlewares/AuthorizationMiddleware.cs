using System.Threading.Tasks;

using DataAccessLayer.DatabasesManager.Interfaces;

using Microsoft.AspNetCore.Http;


namespace DocumentsAPI.Middlewares
{
    public class AuthorizationMiddleware
    {
        private readonly RequestDelegate nextMiddleware;

        public AuthorizationMiddleware(RequestDelegate nextMiddleware, IDatabaseManager databaseManager)
        {
            this.nextMiddleware = nextMiddleware;
        }


        public async Task InvokeAsync(HttpContext context)
        {
            var authHeader = context.Request.Headers["Authorization"].ToString();
            
            await nextMiddleware.Invoke(context).ConfigureAwait(false);
        }
    }
}