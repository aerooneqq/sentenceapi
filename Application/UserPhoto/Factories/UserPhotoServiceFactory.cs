using DataAccessLayer.DatabasesManager.Interfaces;
using SentenceAPI.Features.UserPhoto.Interfaces;
using SentenceAPI.Features.UserPhoto.Services;
using SharedLibrary.FactoriesManager.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentenceAPI.Features.UserPhoto.Factories
{
    class UserPhotoServiceFactory : IUserPhotoServiceFactory
    {
        public IUserPhotoService GetService(IFactoriesManager factoriesManager, IDatabaseManager databasesManager)
        {
            return new UserPhotoService(factoriesManager, databasesManager);
        }
    }
}
