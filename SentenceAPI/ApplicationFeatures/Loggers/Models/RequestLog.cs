using System.IO;
using System.Text;

using Microsoft.AspNetCore.Http;

using MongoDB.Bson.Serialization.Attributes;

using Newtonsoft.Json;


namespace SentenceAPI.ApplicationFeatures.Loggers.Models
{
    public class RequestLog
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
            // using (StreamReader sr = new StreamReader(request.Body, Encoding.UTF8, true, 1024, true))
            // {
            //     Body = sr.ReadLineAsync().GetAwaiter().GetResult();
            // }

            // request.Body.Seek(0, SeekOrigin.Begin);

            QueryString = request.QueryString.Value;
            ContentType = request.ContentType;
            ContentLength = request.ContentLength;
            Host = new HostObject(request.Host);
            IsHttps = request.IsHttps;
            Method = request.Method;
        }
    }
}