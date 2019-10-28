using SharedLibrary.ActionResults.ResponseBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Serialization;
using SharedLibrary.Serialization.Json;

namespace SharedLibrary.ActionResults
{
    public class BadSendedRequest<T> : IActionResult    
    {
        private IResponseBuilder responseBuilder;
        private ISerializer<T> serializer;
        private T message;

        #region Constructors
        public BadSendedRequest() { }

        public BadSendedRequest(T message)
        {
            this.message = message;
            this.serializer = new JsonSerializer<T>(this.message);
        }
        #endregion

        public Task ExecuteResultAsync(ActionContext context)
        {
            return Task.Run(() =>
            {
                responseBuilder = new HttpResponseBuilder(context.HttpContext.Response, Encoding.UTF8);

                responseBuilder.SetCORSHeaders().SetStatusCode((int)HttpReturnCodes.BadSendedRequest);

                if (message != null)
                {
                    responseBuilder.SetContent(serializer.Serialize()).SetContentLength();
                }
            });
        }
    }
}
