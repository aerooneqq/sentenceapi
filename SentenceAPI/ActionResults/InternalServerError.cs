using Microsoft.AspNetCore.Mvc;
using SentenceAPI.ActionResults.ResponseBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentenceAPI.ActionResults
{
    public class InternalServerError : IActionResult
    {
        private readonly string message;
        private readonly Encoding encoding;

        private IResponseBuilder responseBuilder;

        #region Constructors
        public InternalServerError(string message, Encoding encoding)
        {
            this.message = message;
            this.encoding = encoding;
        }

        public InternalServerError(string message) : this(message, Encoding.UTF8) { }

        public InternalServerError() : this(string.Empty, Encoding.UTF8) { }
        #endregion

        public Task ExecuteResultAsync(ActionContext context)
        {
            return Task.Run(() =>
            {
                responseBuilder = new HttpResponseBuilder(context.HttpContext.Response, encoding);

                responseBuilder.SetCORSHeaders().SetStatusCode((int)HttpResponseCodes.InternalServerError)
                               .SetContentType(ContentTypes.TextPlain)
                               .SetContent(message)
                               .SetContentLength();
            });
        }
    }
}
