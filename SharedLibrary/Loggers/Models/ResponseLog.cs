using System.IO;
using System.Text;

using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace SharedLibrary.Loggers.Models
{
    public class ResponseLog
    {
        [JsonProperty("ContentLength")]
        public long ContentLength { get; set; }

        [JsonProperty("StatusCode")]
        public int StatusCode { get; set; }

        [JsonProperty("ContentType")]
        public string ContentType { get; set; }

        [JsonProperty("Body")]
        public string Body { get; set; }

        [JsonProperty("Headers")]
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