using Application.UserPhoto.Interfaces;
using DataAccessLayer.DatabasesManager.Interfaces;
using SentenceAPI.Features.UserPhoto.Services;

using SharedLibrary.FactoriesManager.Interfaces;


namespace Application.UserPhoto.Factories
{
    public class UserPhotoServiceFactory : IUserPhotoServiceFactory
    {
        public IUserPhotoService GetService(IFactoriesManager factoriesManager, IDatabaseManager databasesManager)
        {
            return new UserPhotoService(factoriesManager, databasesManager);
        }
    }
}
