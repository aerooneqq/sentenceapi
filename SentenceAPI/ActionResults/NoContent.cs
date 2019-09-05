using Microsoft.AspNetCore.Mvc;

using SentenceAPI.ActionResults.ResponseBuilder;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentenceAPI.ActionResults
{
    public class NoContent : IActionResult
    {
        private IResponseBuilder responseBuilder;

        public Task ExecuteResultAsync(ActionContext actionContext)
        {
            return Task.Run(() =>
            {
                responseBuilder = new HttpResponseBuilder(actionContext.HttpContext.Response, Encoding.UTF8);

                responseBuilder.SetStatusCode((int)HttpResponseCodes.NoContent)
                               .SetCORSHeaders();
            });
        }
    }
}
