using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccessLayer.DatabasesManager.Interfaces;
using SentenceAPI.Features.UserActivity.Interfaces;
using SentenceAPI.Features.UserActivity.Services;

namespace SentenceAPI.Features.UserActivity.Factories
{
    public class UserActivityServiceFactory : IUserActivityServiceFactory
    {
        public IUserActivityService GetService(IDatabaseManager databasesManager)
        {
            return new UserActivityService(databasesManager);
        }
    }
}
