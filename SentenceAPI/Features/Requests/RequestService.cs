using Microsoft.AspNetCore.Http;
using SentenceAPI.Features.Requests.Interfaces;
using SentenceAPI.Serialization;
using SentenceAPI.Serialization.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentenceAPI.Features.Requests
{
    public class RequestService : IRequestService
    {
        public string GetRequestBody(HttpRequest request)
        {
            using (StreamReader sr = new StreamReader(request.Body, Encoding.UTF8, true, 1024, true))
            {
                return sr.ReadToEnd();
            }
        }

        public T GetRequestBody<T>(HttpRequest request)
        {
            using (StreamReader sr = new StreamReader(request.Body, Encoding.UTF8, true, 1024, true))
            {
                string body = sr.ReadToEnd();

                IDeserializer<T> deserializer = new JsonDeserializer<T>(body);
                return deserializer.Deserialize();
            }
        }

        public string GetToken(HttpRequest request)
        {
            string authHeader = request.Headers["Authorization"];
            return authHeader.Split()[1];
        }
    }
}
