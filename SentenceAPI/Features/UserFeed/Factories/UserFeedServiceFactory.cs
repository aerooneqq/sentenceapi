using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SentenceAPI.Features.UserFeed.Interfaces;
using SentenceAPI.Features.UserFeed.Services;

namespace SentenceAPI.Features.UserFeed.Factories
{
    public class UserFeedServiceFactory : IUserFeedServiceFactory
    {
        public IUserFeedService GetService()
        {
            return new UserFeedService();
        }
    }
}
