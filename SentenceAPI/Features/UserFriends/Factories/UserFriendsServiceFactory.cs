using DataAccessLayer.DatabasesManager.Interfaces;
using SentenceAPI.Features.UserFriends.Interfaces;
using SentenceAPI.Features.UserFriends.Services;
using SharedLibrary.FactoriesManager.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentenceAPI.Features.UserFriends.Factories
{
    public class UserFriendsServiceFactory : IUserFriendsServiceFactory
    {
        public IUserFriendsService GetSerivce(IFactoriesManager factoriesManager, IDatabaseManager databasesManager)
        {
            return new UserFriendsService(factoriesManager, databasesManager);
        }
    }
}
