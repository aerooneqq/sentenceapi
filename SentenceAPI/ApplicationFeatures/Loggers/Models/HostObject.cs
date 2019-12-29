using Microsoft.AspNetCore.Http;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace SentenceAPI.ApplicationFeatures.Loggers.Models
{
    public class HostObject
    {
        [BsonElement("port"), JsonProperty("port")]
        public int? Port { get; set; }

        [BsonElement("host"), JsonProperty("host")]
        public string Host { get; set; }

        public HostObject(HostString host)
        {
            Port = host.Port;
            Host = host.Host;
        }
    }
}