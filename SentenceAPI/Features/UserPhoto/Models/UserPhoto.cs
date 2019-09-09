using DataAccessLayer.KernelModels;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentenceAPI.Features.UserPhoto.Models
{
    public class UserPhoto : UniqueEntity
    {
        [BsonElement("userID"), JsonProperty("userID")]
        public long UserID { get; set; }

        [BsonElement("photo"), JsonProperty("photo")]
        public string Photo { get; set; }

        #region Constructors
        public UserPhoto() { }

        public UserPhoto(long userID, byte[] photo)
            :this(userID, Convert.ToBase64String(photo)) { }

        public UserPhoto(long userID, string photo)
        {
            UserID = userID;
            Photo = photo;
        }
        #endregion
    }
}
