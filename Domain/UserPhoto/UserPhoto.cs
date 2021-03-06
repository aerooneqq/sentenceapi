﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;

using Domain.JsonConverters;
using Domain.KernelModels;


namespace Domain.UserPhoto
{
    public class UserPhoto : UniqueEntity
    {
        #region Properties
        [BsonElement("userID"), JsonProperty("userID"), JsonConverter(typeof(ObjectIDJsonConverter))]
        public ObjectId UserID { get; set; }

        [BsonElement("gridFSPhotoes"), JsonProperty("gridFSPhotoes")]
        public Dictionary<string, string> GridFSPhotoes { get; set; }

        [BsonElement("fileName"), JsonProperty("fileName")]
        public string FileName { get; set; }

        [BsonElement("updatedAt"), JsonProperty("updatedAt")]
        public DateTime UpdatedAt { get; set; }

        [BsonElement("currentPhotoPointer"), JsonProperty("currentPhotoPointer")]
        [JsonConverter(typeof(ObjectIDJsonConverter))]
        public ObjectId CurrentPhotoID { get; set; }
        #endregion

        
        #region Constructors
        public UserPhoto() 
        { 
            GridFSPhotoes = new Dictionary<string, string>();
        }

        public UserPhoto(ObjectId userID) : this()
        { 
            UserID = userID;
        }
        #endregion
    }
}
