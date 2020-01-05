using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using SentenceAPI.Features.Users.Models;
using DataAccessLayer.KernelModels;
using MongoDB.Bson;

namespace SentenceAPI.Features.Links.Models
{
    public class VerificationLink : UniqueEntity
    {
        #region Static Properties
        public static Random Random { get; } = new Random();
        #endregion

        #region Model properties
        [BsonElement("link"), JsonProperty("link")]
        public string Link { get; set; }
        
        [BsonElement("used"), JsonProperty("used")]
        public bool Used { get; set; }

        [BsonElement("creationDate"), JsonProperty("creationDate")]
        public DateTime CreationDate { get; set; }

        [BsonElement("useDate"), JsonProperty("useDate")]
        public DateTime? UseDate { get; set; }

        [BsonElement("userID"), JsonProperty("userID")]
        public ObjectId UserID { get; set; } 

        [BsonElement("hashCode"), JsonProperty("hashCode")]
        public string HashCode { get; set; }
        #endregion

        public VerificationLink(UserInfo user)
        {
            CreationDate = DateTime.UtcNow;
            Used = false;
            UseDate = null;
            UserID = user.ID;
            HashCode = Random.Next().GetHashCode().ToString();
            Link = $"https://localhost:44368/api/links/{HashCode}";
        }
    }
}
