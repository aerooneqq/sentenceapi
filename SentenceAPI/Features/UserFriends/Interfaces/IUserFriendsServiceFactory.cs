using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SharedLibrary.KernelInterfaces;

namespace SentenceAPI.Features.UserFriends.Interfaces
{
    public interface IUserFriendsServiceFactory : IServiceFactory
    {
        IUserFriendsService GetSerivce();
    }
}
