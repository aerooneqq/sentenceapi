using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentenceAPI.Features.Workplace.DocumentsDeskState.Models
{
    public class DocumentTopBarInfo
    {
        [BsonElement("documentID"), JsonProperty("documentID")]
        public long DocumentID { get; set; }

        [BsonElement("documentName"), JsonProperty("documentName")]
        public string DocumentName { get; set; }
    }
}
