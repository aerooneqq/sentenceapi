using System.IO;
using System.Text;

using Microsoft.AspNetCore.Http;

using MongoDB.Bson.Serialization.Attributes;

using Newtonsoft.Json;

using Domain.KernelModels;


namespace Domain.Logs
{
    public class RequestLog : UniqueEntity
    {
        [JsonProperty("queryString"), BsonElement("queryString")]
        public string QueryString { get; set; }

        [JsonProperty("body"), BsonElement("body")]
        public string Body { get; set; }

        [JsonProperty("contentType"), BsonElement("contentType")]
        public string ContentType { get; set; }

        [JsonProperty("contentLength"), BsonElement("contentLength")]
        public long? ContentLength { get; set; }

        [JsonProperty("isHttps"), BsonElement("isHttps")]
        public bool IsHttps { get; set; }

        [JsonProperty("host"), BsonElement("host")]
        public HostObject Host { get; set; }

        [JsonProperty("method"), BsonElement("method")]
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