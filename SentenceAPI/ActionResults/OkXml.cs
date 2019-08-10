using Microsoft.AspNetCore.Mvc;
using SentenceAPI.ActionResults.ResponseBuilder;
using SentenceAPI.Serialization;
using SentenceAPI.Serialization.Json;
using SentenceAPI.Serialization.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentenceAPI.ActionResults
{
    public class OkXml<T> : IActionResult
    {
        #region Fields
        private readonly T obj;
        private Encoding encoding;

        private ISerializer<T> serializer;
        private IResponseBuilder responseBuilder;
        #endregion

        #region Constructors
        public OkXml(in T obj, Encoding encoding)
        {
            this.obj = obj;
            this.encoding = encoding;

            serializer = new XmlSerializer<T>(this.obj);
        }

        public OkXml(in T obj) : this(obj, Encoding.UTF8) { }
        #endregion

        public Task ExecuteResultAsync(ActionContext context)
        {
            return Task.Run(() =>
            {
                responseBuilder = new HttpResponseBuilder(context.HttpContext.Response, encoding);
                responseBuilder.SetStatusCode(200)
                               .SetContentType(ContentTypes.ApplicationXml)
                               .SetContent(serializer.Serialize())
                               .SetContentLength();
            });
        }
    }
}
