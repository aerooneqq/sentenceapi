using MongoDB.Bson;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentenceAPI.Features.Users.Models
{
    public class UserSearchResult
    {
        #region Properties
        [JsonProperty("userID")]
        public ObjectId UserID { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("birthDate")]
        public DateTime BirthDate { get; set; }
        #endregion

        #region Constructors
        public UserSearchResult(ObjectId id, string name, DateTime birthDate)
        {
            UserID = id;
            Name = name;
            BirthDate = birthDate;
        }

        public UserSearchResult(UserInfo user) : this(user.ID, user.Name, user.BirthDate) { }
        #endregion
    }
}
