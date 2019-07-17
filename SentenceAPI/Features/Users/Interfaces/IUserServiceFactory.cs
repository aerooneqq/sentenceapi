using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SentenceAPI.Features.Users.Models;
using SentenceAPI.KernelInterfaces;

namespace SentenceAPI.Features.Users.Interfaces
{
    interface IUserServiceFactory : IFactory
    {
        IUserService<UserInfo> GetService();
    }
}
