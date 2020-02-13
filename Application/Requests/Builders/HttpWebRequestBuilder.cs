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
        public HttpWebRequestBuilder(HttpRequest httpRequest)
        {
            this.httpRequest = httpRequest;
            HttpWebRequest = WebRequest.CreateHttp(httpRequest.Path.ToUriComponent());
        }

        /// <summary>
        /// Copies all headers from the httpRequest and adds the ID header for this request. 
        /// </summary>
        public HttpWebRequestBuilder SetHeaders(ObjectId requestId)
        {
            HttpWebRequest.Headers = new WebHeaderCollection();
            
            foreach ((string headerKey, StringValues headerValue) in httpRequest.Headers)
            {
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
            using StreamReader sr = new StreamReader(httpRequest.Body);
            string httpRequestContent = await sr.ReadToEndAsync();
            
            byte[] httpRequestByteContent = Encoding.UTF8.GetBytes(httpRequestContent);
            
            await (await HttpWebRequest.GetRequestStreamAsync()).WriteAsync(httpRequestByteContent);

            HttpWebRequest.ContentLength = httpRequestContent.Length;
            HttpWebRequest.ContentType = httpRequest.ContentType;

            return this;
        }
    }
}