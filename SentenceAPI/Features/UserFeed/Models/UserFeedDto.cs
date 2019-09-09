using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentenceAPI.Features.UserFeed.Models
{
    public class UserFeedDto
    {
        [JsonProperty("userFeed")]
        public UserFeed UserFeed { get; set; }

        [JsonProperty("userPhoto")]
        public List<byte> UserPhoto { get; set; }

        #region Constructors
        public UserFeedDto() { }

        public UserFeedDto(UserFeed userFeed, List<byte> userPhoto)
        {
            UserFeed = userFeed;
            UserPhoto = userPhoto;
        }
        #endregion
    }
}
