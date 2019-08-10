using Microsoft.AspNetCore.Mvc;
using SentenceAPI.ActionResults.ResponseBuilder;
using SentenceAPI.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentenceAPI.ActionResults
{
    public class StatusCode<T> : IActionResult where T : class
    {
        #region Fields
        private readonly T obj;
        private readonly Encoding encoding;
        private readonly int statusCode;

        private IResponseBuilder responseBuilder;
        private ISerializer<T> serializer;
        #endregion

        #region Constructors
        public StatusCode(int statusCode, T obj, Encoding encoding)
        {
            this.statusCode = statusCode;
            this.obj = obj;
            this.encoding = encoding;
        }

        public StatusCode(int statusCode, T obj) : this(statusCode, obj, Encoding.UTF8) { }

        public StatusCode(int statusCode) : this(statusCode, null, Encoding.UTF8) { }
        #endregion

        public Task ExecuteResultAsync(ActionContext context)
        {
            return Task.Run(() =>
            {
                responseBuilder = new HttpResponseBuilder(context.HttpContext.Response, encoding);
                responseBuilder.SetStatusCode(statusCode);

                if (obj != null)
                {
                    responseBuilder.SetContent(serializer.Serialize())
                                   .SetContentLength()
                                   .SetContentType(ContentTypes.ApplicationJson);
                }
            });
        }
    }
}
