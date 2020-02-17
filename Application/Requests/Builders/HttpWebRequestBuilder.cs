using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

using MongoDB.Bson;


namespace Application.Requests
{
    public class HttpWebRequestBuilder
    {
        private readonly HttpRequest httpRequest;
        
        public HttpWebRequest HttpWebRequest { get; }


        /// <summary>
        /// Initializes the webRequest with the URI from the httpRequest
        /// </summary>
        public HttpWebRequestBuilder(HttpRequest httpRequest, string url)
        {
            this.httpRequest = httpRequest;
            
            HttpWebRequest = WebRequest.CreateHttp(url);
        }

        private static string GetHost(string url)
        {
            const int slashCount = 2;

            for (int i = 0; i < slashCount; ++i)
            {
                url = url.Remove(0, url.IndexOf("/", StringComparison.Ordinal) + 1);
            }

            return url.Substring(url.IndexOf("/", StringComparison.Ordinal));
        }

        /// <summary>
        /// Copies all headers from the httpRequest and adds the ID header for this request. 
        /// </summary>
        public HttpWebRequestBuilder SetHeaders(ObjectId requestId)
        {
            HttpWebRequest.Headers = new WebHeaderCollection();
            HttpWebRequest.Method = httpRequest.Method;
            foreach ((string headerKey, StringValues headerValue) in httpRequest.Headers)
            {
                if (headerKey != "Host" && headerKey != "Method")
                    HttpWebRequest.Headers.Add(headerKey, headerValue);
            }
            
            HttpWebRequest.Headers.Add("SentenceAPIRequestId", requestId.ToString());

            return this;
        }

        /// <summary>
        /// Sets the content for the HttpWebRequest, content type and content length
        /// </summary>
        public async Task<HttpWebRequestBuilder> SetContent()
        {
            //Body for get methods are not supported
            if (httpRequest.Method == "GET") 
                return this;
            
            using StreamReader sr = new StreamReader(httpRequest.Body);
            string httpRequestContent = await sr.ReadToEndAsync();
            byte[] httpRequestByteContent = Encoding.UTF8.GetBytes(httpRequestContent);

            HttpWebRequest.ContentLength = httpRequestContent.Length;
            HttpWebRequest.ContentType = httpRequest.ContentType;

            await (await HttpWebRequest.GetRequestStreamAsync()).WriteAsync(httpRequestByteContent, 
                0, httpRequestByteContent.Length);

            return this;
        }
    }
}