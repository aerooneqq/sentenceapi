using Microsoft.AspNetCore.Http;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace SharedLibrary.Loggers.Models
{
    public class HostObject
    {
        public int? Port { get; set; }

        public string Host { get; set; }

        public HostObject(HostString host)
        {
            Port = host.Port;
            Host = host.Host;
        }
    }
}