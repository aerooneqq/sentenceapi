using DataAccessLayer.KernelModels;
using MongoDB.Bson;
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

        [BsonElement("photoGridFSId"), JsonProperty("photoGridFSId")]
        public ObjectId PhotoGridFSId { get; set; }

        [BsonElement("fileName"), JsonProperty("fileName")]
        public string FileName { get; set; }

        #region Constructors
        public UserPhoto() { }

        public UserPhoto(long userID) : this(userID, ObjectId.Empty) { }
        public UserPhoto(long userID, ObjectId objectId)
        { 
            UserID = userID;
            PhotoGridFSId = objectId;
        }
        #endregion
    }
}
