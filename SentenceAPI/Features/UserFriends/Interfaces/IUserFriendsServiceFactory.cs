using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SentenceAPI.KernelInterfaces;

namespace SentenceAPI.Features.UserFriends.Interfaces
{
    public interface IUserFriendsServiceFactory : IFactory
    {
        IUserFriendsService GetSerivce();
    }
}
