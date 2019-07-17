using System.Collections.Generic;
using System.Linq;

using SentenceAPI.Features.Users.Interfaces;
using SentenceAPI.Features.Users.Models;

namespace SentenceAPI.Features.Users.Services
{
    public class UserService : IUserService<UserInfo>
    {
        private List<UserInfo> users = new List<UserInfo>()
        {
            new UserInfo()
            {
                ID = 0,
                Email = "aerooneQ@yandex.ru",
                Login = "Aero",
                Password = "Aerooen123"
            }
        };

        public void Delete(long id)
        {
            throw new System.NotImplementedException();
        }

        public UserInfo Get(string email, string password)
        {
            return users.FirstOrDefault(u => u.Email == email && u.Password == password);
        }

        public void Insert(UserInfo user)
        {
            throw new System.NotImplementedException();
        }

        public void Update(UserInfo user)
        {
            throw new System.NotImplementedException();
        }
    }
}
