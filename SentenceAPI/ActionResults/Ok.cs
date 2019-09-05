using Microsoft.AspNetCore.Mvc;
using SentenceAPI.ActionResults.ResponseBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentenceAPI.ActionResults
{
    public class Ok : IActionResult
    {
        private IResponseBuilder responseBuilder;
        private string message;

        #region Constructors
        public Ok(string message)
        {
            this.message = message;
        }

        public Ok()
        {

        }
        #endregion

        public Task ExecuteResultAsync(ActionContext context)
        {
            return Task.Run((() =>
            {
                responseBuilder = new HttpResponseBuilder(context.HttpContext.Response, Encoding.UTF8);

                responseBuilder.SetStatusCode((int)HttpResponseCodes.OK).SetCORSHeaders(); 

                if (message != null)
                {
                    responseBuilder.SetContent(message).SetContentLength();
                }
            }));
        }
    }
}
