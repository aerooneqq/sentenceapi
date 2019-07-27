using System.Collections.Generic;

using Newtonsoft.Json;

using SentenceAPI.KernelModels;

using MongoDB.Bson.Serialization.Attributes;
using System;

namespace SentenceAPI.Features.Users.Models
{
    public class UserInfo : UniqueEntity
    {
        #region Authentication 
        [BsonElement("login"), JsonProperty("login")]
        public string Login { get; set; }

        [BsonElement("email"), JsonProperty("email")]
        public string Email { get; set; }

        [BsonElement("password"), JsonProperty("password")]
        public string Password { get; set; }
        #endregion

        #region User data (name + country + photo)
        [BsonElement("name"), JsonProperty("name")]
        public string Name { get; set; }

        [BsonElement("surname"), JsonProperty("surname")]
        public string Surname { get; set; }

        [BsonElement("middleName"), JsonProperty("middleName")]
        public string MiddleName { get; set; }

        [BsonElement("country"), JsonProperty("country")]
        public string Country { get; set; }

        [BsonElement("city"), JsonProperty("city")]
        public string City { get; set; }

        [BsonElement("photo"), JsonProperty("photo")]
        public byte[] Photo { get; set; }
        #endregion

        [BsonElement("birthDate"), JsonProperty("birthDate")]
        public DateTime BirthDate { get; set; } 

        #region Career
        [BsonElement("careerStages"), JsonProperty("careerStages")]
        public List<CareerStage> CareerStages { get; set; }
        #endregion

        #region System properties
        [BsonElement("isAccountVerified"), JsonProperty("isAccountVerified")]
        public bool IsAccountVerified { get; set; }
        #endregion
    }
}
