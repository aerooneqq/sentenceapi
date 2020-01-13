using Microsoft.AspNetCore.Mvc;
using SharedLibrary.ActionResults.ResponseBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.ActionResults
{
    public class Unauthorized : IActionResult
    {
        private IResponseBuilder responseBuilder;

        public Task ExecuteResultAsync(ActionContext context)
        {
            return Task.Run(() =>
            {
                responseBuilder = new HttpResponseBuilder(context.HttpContext.Response, Encoding.UTF8);

                responseBuilder.SetStatusCode((int)HttpReturnCodes.Unauthorized)
                               .SetCORSHeaders();
            });
        }
    }
}
