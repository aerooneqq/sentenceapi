using SentenceAPI.KernelInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentenceAPI.Features.UserFeed.Interfaces
{
    public interface IUserFeedServiceFactory : IFactory
    {
        IUserFeedService GetService();
    }
}
