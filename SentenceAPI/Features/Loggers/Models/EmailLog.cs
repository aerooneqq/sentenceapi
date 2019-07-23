using SentenceAPI.KernelModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MongoDB.Bson.Serialization.Attributes;

using Newtonsoft.Json;
using SentenceAPI.Features.Users.Models;

namespace SentenceAPI.Features.Loggers.Models
{
    public class EmailLog : UniqueEntity
    {
        [BsonElement("emailAddress"), JsonProperty("emailAddress")]
        public string EmailAddress { get; set; }

        [BsonElement("sendDate"), JsonProperty("sendDate")]
        public DateTime SendDate { get; set; }

        [BsonElement("text"), JsonProperty("text")]
        public string Text { get; set; }

        [BsonElement("configuration"), JsonProperty("configuration")]
        public LogConfiguration LogConfiguration { get; set; }

        public EmailLog(UserInfo user, string text)
        {
            EmailAddress = user.Email;
            Text = text;
            SendDate = DateTime.UtcNow;
        }
    }
}
