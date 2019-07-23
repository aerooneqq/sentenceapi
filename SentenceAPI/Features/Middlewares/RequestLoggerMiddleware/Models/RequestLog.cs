using SentenceAPI.KernelModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

using MongoDB.Bson.Serialization.Attributes;
using Microsoft.AspNetCore.Http;

using Newtonsoft.Json;

namespace SentenceAPI.Features.Middlewares.RequestLoggerMiddleware.Models
{
    public class RequestLog : UniqueEntity
    {
        [BsonElement("queryString"), JsonProperty("queryString")]
        public string QueryString { get; set; }

        [BsonElement("body"), JsonProperty("body")]
        public string Body { get; set; }

        [BsonElement("contentType"), JsonProperty("contentType")]
        public string ContentType { get; set; }

        [BsonElement("contentLength"), JsonProperty("contentLength")]
        public long? ContentLength { get; set; }

        [BsonElement("isHttps"), JsonProperty("isHttps")]
        public bool IsHttps { get; set; }

        [BsonElement("host"), JsonProperty("host")]
        public HostObject Host { get; set; }

        [BsonElement("method"), JsonProperty("method")]
        public string Method { get; set; }

        public RequestLog(HttpRequest request)
        {
            QueryString = request.QueryString.Value;

            using (StreamReader sr = new StreamReader(request.Body))
            {
                Body = sr.ReadToEnd();
            }

            ContentType = request.ContentType;
            ContentLength = request.ContentLength;
            Host = new HostObject(request.Host);
            IsHttps = request.IsHttps;
            Method = request.Method;
        }
    }
}
