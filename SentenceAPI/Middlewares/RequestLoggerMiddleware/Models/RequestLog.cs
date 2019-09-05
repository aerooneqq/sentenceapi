using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Text;

using MongoDB.Bson.Serialization.Attributes;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;

using Newtonsoft.Json;

using DataAccessLayer.KernelModels;

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
            request.EnableRewind();
            using (StreamReader sr = new StreamReader(request.Body, Encoding.UTF8, true, 1024, true))
            {
                Body = sr.ReadToEnd();
            }

            request.Body.Position = 0;

            QueryString = request.QueryString.Value;
            ContentType = request.ContentType;
            ContentLength = request.ContentLength;
            Host = new HostObject(request.Host);
            IsHttps = request.IsHttps;
            Method = request.Method;
        }
    }
}
