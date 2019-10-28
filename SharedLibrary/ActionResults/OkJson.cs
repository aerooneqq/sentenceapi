using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.IO;

using SharedLibrary.Serialization;
using SharedLibrary.Serialization.Json;
using SharedLibrary.ActionResults.ResponseBuilder;

namespace SharedLibrary.ActionResults
{
    public class OkJson<T> : IActionResult
    {
        #region Fields
        private readonly T obj;
        private Encoding encoding;

        private ISerializer<T> serializer;
        private IResponseBuilder responseBuilder;
        #endregion

        #region Constructors
        public OkJson(in T obj, Encoding encoding)
        {
            this.obj = obj;
            this.encoding = encoding;

            serializer = new JsonSerializer<T>(this.obj);
        }

        public OkJson(in T obj) : this(obj, Encoding.UTF8) { }
        #endregion

        public Task ExecuteResultAsync(ActionContext context)
        {
            return Task.Run(() => 
            {
                responseBuilder = new HttpResponseBuilder(context.HttpContext.Response, encoding);

                responseBuilder.SetCORSHeaders()
                               .SetStatusCode((int)HttpReturnCodes.OK)
                               .SetContentType(ContentTypes.ApplicationJson)
                               .SetContent(serializer.Serialize())
                               .SetContentLength();
            });
        }
    }
}
