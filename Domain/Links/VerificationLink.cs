using System;

using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

using Newtonsoft.Json;

using Domain.Users;
using Domain.KernelModels;


namespace Domain.Links
{
    public class VerificationLink : UniqueEntity
    {
        #region Static Properties
        private static Random Random { get; } = new Random();
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
