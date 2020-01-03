using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SentenceAPI.Features.Users.Interfaces;
using SentenceAPI.Features.Users.Services;
using SentenceAPI.Features.Users.Models;

using SharedLibrary.KernelInterfaces;
using SharedLibrary.FactoriesManager.Interfaces;
using DataAccessLayer.DatabasesManager.Interfaces;

namespace SentenceAPI.Features.Users.Factories
{
    public class UserServiceFactory : IUserServiceFactory
    {
        public IUserService<UserInfo> GetService(IFactoriesManager factoriesManager, 
                                                 IDatabaseManager databasesManager)
        {
            return new UserService(factoriesManager, databasesManager);
        }
    }
}
