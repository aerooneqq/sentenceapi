using System.IO;
using System.Text;

using Microsoft.AspNetCore.Http;

using MongoDB.Bson.Serialization.Attributes;

using Newtonsoft.Json;


namespace SharedLibrary.Loggers.Models
{
    public class RequestLog
    {
        public string QueryString { get; set; }

        public string Body { get; set; }

        public string ContentType { get; set; }

        public long? ContentLength { get; set; }

        public bool IsHttps { get; set; }

        public HostObject Host { get; set; }
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