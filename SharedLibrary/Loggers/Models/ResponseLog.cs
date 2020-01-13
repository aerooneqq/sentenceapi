using System.IO;
using System.Text;

using Microsoft.AspNetCore.Http;

using MongoDB.Bson.Serialization.Attributes;

using Newtonsoft.Json;


namespace SharedLibrary.Loggers.Models
{
    public class ResponseLog
    {
        [JsonProperty("ContentLength"), BsonElement("contentLength")]
        public long ContentLength { get; set; }

        [JsonProperty("StatusCode"), BsonElement("statusCode")]
        public int StatusCode { get; set; }

        [JsonProperty("ContentType"), BsonElement("contentType")]
        public string ContentType { get; set; }

        [JsonProperty("Body"), BsonElement("body")]
        public string Body { get; set; }

        [JsonProperty("Headers"), BsonElement("headers")]
        public IHeaderDictionary Headers { get; set; }

        public ResponseLog(HttpResponse response) 
        {
            ContentLength = response.ContentLength ?? 0;
            StatusCode = response.StatusCode;
            ContentType = response.ContentType;
            Headers = response.Headers;

            // using (StreamReader sr = new StreamReader(response.Body, Encoding.UTF8, true, 1024, true))
            // {
            //     Body =  sr.ReadLineAsync().GetAwaiter().GetResult();
            // }

            // response.Body.Position = 0;
        }
    }
}