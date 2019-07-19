using System.Collections.Generic;

using SentenceAPI.KernelModels;

namespace SentenceAPI.Features.Users.Models
{
    public class UserInfo : UniqueEntity
    {
        #region Authentication 
        public string Login { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        #endregion

        #region User data (name + country)
        public string Name { get; set; }
        public string Surname { get; set; }
        public string MiddleName { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        #endregion

        #region Career
        public List<CareerStage> CareerStages { get; set; }
        #endregion
    }
}
