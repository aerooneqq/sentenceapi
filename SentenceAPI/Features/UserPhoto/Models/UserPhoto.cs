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
        #region Properties
        [BsonElement("userID"), JsonProperty("userID")]
        public long UserID { get; set; }

        [BsonElement("gridFSPhotoes"), JsonProperty("gridFSPhotoes")]
        public Dictionary<string, string> GridFSPhotoes { get; set; }

        [BsonElement("fileName"), JsonProperty("fileName")]
        public string FileName { get; set; }

        [BsonElement("updatedAt"), JsonProperty("updatedAt")]
        public DateTime UpdatedAt { get; set; }

        [BsonElement("currentPhotoPointer"), JsonProperty("currentPhotoPointer")]
        public ObjectId CurrentPhotoID { get; set; }
        #endregion

        #region Constructors
        public UserPhoto() 
        { 
            GridFSPhotoes = new Dictionary<string, string>();
        }

        public UserPhoto(long userID) : this()
        { 
            UserID = userID;
        }
        #endregion
    }
}
