﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
 
 using Domain.KernelModels;

 
namespace Domain.Logs
{
    public class ApplicationError : UniqueEntity
    {
        [BsonElement("errorDate"), JsonProperty("errorDate")]
        public DateTime ErrorDate { get; set; }

        [BsonElement("message"), JsonProperty("message")]
        public string Message { get; set; }

        [BsonElement("configuration"), JsonProperty("configuration")]
        public LogConfiguration LogConfiguration { get; set; }

        [BsonElement("stackTrace"), JsonProperty("stackTrace")]
        public string StackTrace { get; set; }

        [BsonElement("source"), JsonProperty("source")]
        public string Source { get; set; }


        #region Constructors
        public ApplicationError(Exception ex)
        {
            ErrorDate = DateTime.UtcNow;
            Message = ex.Message;
            StackTrace = ex.StackTrace;
            Source = ex.Source;
        }
        #endregion
    }
}
