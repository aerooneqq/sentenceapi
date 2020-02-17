using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Application.Responses.Interfaces;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;

namespace Application.Responses
{
    public class ResponseCopier : IResponseCopier
    {
        private HttpWebResponse webResponse;
        private HttpResponse response;
        
        public IResponseCopier Copy(HttpWebResponse response)
        {
            this.webResponse = response;
            
            return this;
        }

        public async Task To(HttpResponse response)
        {
            this.response = response;

            response.OnStarting((state) => 
            {
                HttpResponse httpResponse = (HttpResponse)state;

                httpResponse.Headers.Add("Access-Control-Allow-Origin", "*");
                httpResponse.Headers.Add("Access-Control-Allow-Methods", "*");
                httpResponse.Headers.Add("Access-Control-Allow-Headers", "*");

                httpResponse.ContentType = webResponse.ContentType;
                httpResponse.StatusCode = (int)webResponse.StatusCode;

                System.Console.WriteLine(httpResponse.ContentType);

                return Task.CompletedTask;
            }, response);

            await CopyContent();
        }

        private async Task CopyContent()
        {
            using StreamReader sr = new StreamReader(webResponse.GetResponseStream());

            string content = await sr.ReadToEndAsync();
            await response.WriteAsync(content);
        }

        private void CopyHeaders()
        {
            foreach (string header in webResponse.Headers.Keys)
            {
                response.Headers.Add(header, webResponse.Headers[header]);
            }
        }

        private void CopyStatusCode()
        {
            response.StatusCode = (int) webResponse.StatusCode;
        }

        private void CopyCookies()
        {
            foreach (string key in webResponse.Cookies)
            {
                response.Cookies.Append(key, webResponse.Cookies[key]?.Value);
            }
        }
    }
}