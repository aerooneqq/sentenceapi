using Microsoft.AspNetCore.Mvc;
using SharedLibrary.ActionResults.ResponseBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.ActionResults
{
    public class Created : IActionResult
    {
        private readonly string content;
        
        public Created() {}
        public Created(string content) => this.content = content;
        
        
        public Task ExecuteResultAsync(ActionContext actionContext)
        {
            return Task.Run(() =>
            {
                IResponseBuilder responseBuilder = new HttpResponseBuilder(actionContext.HttpContext.Response,
                    Encoding.UTF8);

                responseBuilder.SetStatusCode((int)HttpReturnCodes.Created)
                               .SetContentLength(0).SetCORSHeaders();

                if (content is {})
                    responseBuilder.SetContent(content).SetContentLength();
            });
        }

    }
}
