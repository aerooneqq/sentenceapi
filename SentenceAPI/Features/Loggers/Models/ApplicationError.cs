using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SentenceAPI.KernelModels;

using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace SentenceAPI.Features.Loggers.Models
{
    public class ApplicationError : UniqueEntity
    {
        [BsonElement("errorDate"), JsonProperty("errorDate")]
        public DateTime ErrorDate { get; set; }

        [BsonElement("message"), JsonProperty("message")]
        public string Message { get; set; }

        [BsonElement("configuration"), JsonProperty("configuration")]
        public LogConfiguration LogConfiguration { get; set; }

        public ApplicationError(string message, LogConfiguration logConfiguration)
        {
            ErrorDate = DateTime.UtcNow;
            Message = message;
            LogConfiguration = logConfiguration;
        }
    }
}
