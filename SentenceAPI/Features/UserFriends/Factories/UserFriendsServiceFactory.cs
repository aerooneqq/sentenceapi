using SentenceAPI.Features.UserFriends.Interfaces;
using SentenceAPI.Features.UserFriends.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentenceAPI.Features.UserFriends.Factories
{
    public class UserFriendsServiceFactory : IUserFriendsServiceFactory
    {
        public IUserFriendsService GetSerivce()
        {
            return new UserFriendsService();
        }
    }
}
