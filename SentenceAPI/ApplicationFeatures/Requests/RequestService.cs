using Microsoft.AspNetCore.Http;
using SentenceAPI.ApplicationFeatures.Requests.Interfaces;
using SentenceAPI.Serialization;
using SentenceAPI.Serialization.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentenceAPI.ApplicationFeatures.Requests
{
    public class RequestService : IRequestService
    {
        public async Task<string> GetRequestBody(HttpRequest request)
        {
            using (StreamReader sr = new StreamReader(request.Body, Encoding.UTF8, true, 1024, true))
            {
                return await sr.ReadToEndAsync();
            }
        }

        public async Task<T> GetRequestBody<T>(HttpRequest request)
        { 
            using StreamReader sr = new StreamReader(request.Body, Encoding.UTF8, true, 1024, true);
            string body = await sr.ReadToEndAsync().ConfigureAwait(false);

            IDeserializer<T> deserializer = new JsonDeserializer<T>(body);
            return deserializer.Deserialize();
        }

        public string GetToken(HttpRequest request)
        {
            string authHeader = request.Headers["Authorization"];
            return authHeader.Split()[1];
        }
    }
}
