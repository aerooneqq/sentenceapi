using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentenceAPI.Features.Loggers.Models
{
    public class LogConfiguration
    {
        [BsonElement("controllerName"), JsonProperty("controllerName")]
        public string ControllerName { get; set; }

        [BsonElement("serviceName"), JsonProperty("serviceName")]
        public string ServiceName { get; set; }
    }
}
