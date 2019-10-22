using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Newtonsoft.Json;

using MongoDB.Bson.Serialization.Attributes;
using Microsoft.AspNetCore.Http;

namespace SharedLibrary.Middlewares.RequestLogger.Models
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
