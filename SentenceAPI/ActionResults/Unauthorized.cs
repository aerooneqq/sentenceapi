using Microsoft.AspNetCore.Mvc;
using SentenceAPI.ActionResults.ResponseBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentenceAPI.ActionResults
{
    public class Unauthorized : IActionResult
    {
        private IResponseBuilder responseBuilder;

        public Task ExecuteResultAsync(ActionContext context)
        {
            return Task.Run(() =>
            {
                responseBuilder = new HttpResponseBuilder(context.HttpContext.Response, Encoding.UTF8);

                responseBuilder.SetStatusCode((int)HttpResponseCodes.Unauthorized)
                               .SetCORSHeaders();
            });
        }
    }
}
