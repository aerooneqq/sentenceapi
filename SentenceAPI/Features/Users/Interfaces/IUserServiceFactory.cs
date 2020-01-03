using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccessLayer.DatabasesManager.Interfaces;
using SentenceAPI.Features.Users.Models;
using SharedLibrary.FactoriesManager.Interfaces;
using SharedLibrary.KernelInterfaces;

namespace SentenceAPI.Features.Users.Interfaces
{
    public interface IUserServiceFactory : IServiceFactory
    {
        IUserService<UserInfo> GetService(IFactoriesManager factoriesManager, IDatabaseManager databasesManager);
    }
}
