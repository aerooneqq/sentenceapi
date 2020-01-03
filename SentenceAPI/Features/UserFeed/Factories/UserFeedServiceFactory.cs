using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccessLayer.DatabasesManager.Interfaces;
using SentenceAPI.Features.UserFeed.Interfaces;
using SentenceAPI.Features.UserFeed.Services;
using SharedLibrary.FactoriesManager.Interfaces;

namespace SentenceAPI.Features.UserFeed.Factories
{
    public class UserFeedServiceFactory : IUserFeedServiceFactory
    {
        public IUserFeedService GetService(IFactoriesManager factoriesManager, IDatabaseManager databasesManager)
        {
            return new UserFeedService(factoriesManager, databasesManager);
        }
    }
}
