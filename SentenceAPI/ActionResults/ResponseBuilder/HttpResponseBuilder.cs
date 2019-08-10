using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentenceAPI.ActionResults.ResponseBuilder
{
    internal class HttpResponseBuilder : IResponseBuilder, IDisposable
    {
        #region Fields
        private HttpResponse response;
        private Encoding encoding;

        private string content;
        #endregion

        public HttpResponseBuilder(HttpResponse response, Encoding encoding)
        {
            this.response = response;
            this.encoding = encoding;
        }

        #region IResponseBuilder implementation
        public IResponseBuilder SetContent(string content)
        {
            this.content = content;
            using (StreamWriter sr = new StreamWriter(response.Body, encoding))
            {
                sr.Write(content);
            }

            return this;
        }

        public IResponseBuilder SetContentLength(int contentLength)
        {
            response.ContentLength = contentLength;

            return this;
        }

        public IResponseBuilder SetContentLength()
        {
            response.ContentLength = this.content.Length;

            return this;
        }

        public IResponseBuilder SetContentType(string contentType)
        {
            response.ContentType = contentType;

            return this;
        }

        public IResponseBuilder SetStatusCode(int statusCode)
        {
            response.StatusCode = statusCode;

            return this;
        }

        public HttpResponse Build() => response;
        #endregion

        #region iDisppsable implementation
        public void Dispose()
        {
            response = null;
        }
        #endregion
    }
}
