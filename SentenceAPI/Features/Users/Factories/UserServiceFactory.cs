using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SentenceAPI.Features.Users.Interfaces;
using SentenceAPI.Features.Users.Services;
using SentenceAPI.Features.Users.Models;

namespace SentenceAPI.Features.Users.Factories
{
    public class UserServiceFactory : IUserServiceFactory
    {
        public IUserService<UserInfo> GetService()
        {
            return new UserService();
        }
    }
}
