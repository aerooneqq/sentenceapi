using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SharedLibrary.KernelInterfaces;

namespace SentenceAPI.Features.UserFeed.Interfaces
{
    public interface IUserFeedServiceFactory : IServiceFactory
    {
        IUserFeedService GetService();
    }
}
