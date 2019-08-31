using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MongoDB.Bson.Serialization.Attributes;

using Newtonsoft.Json;
using SentenceAPI.Features.Users.Models;
using DataAccessLayer.KernelModels;

namespace SentenceAPI.Features.Loggers.Models
{
    public class EmailLog : UniqueEntity
    {
        #region Public properties
        [BsonElement("emailAddress"), JsonProperty("emailAddress")]
        public string EmailAddress { get; set; }

        [BsonElement("sendDate"), JsonProperty("sendDate")]
        public DateTime SendDate { get; set; }

        [BsonElement("text"), JsonProperty("text")]
        public string Text { get; set; }

        [BsonElement("configuration"), JsonProperty("configuration")]
        public LogConfiguration LogConfiguration { get; set; }
        #endregion

        #region Constructors
        public EmailLog(UserInfo user, string text)
            : this(user.Email, text) { }

        public EmailLog(string email, string text)
        {
            EmailAddress = email;
            Text = text;
            SendDate = DateTime.UtcNow;
        }
        #endregion
    }
}
