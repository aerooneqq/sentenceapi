using SharedLibrary.FactoriesManager.Interfaces;

using DataAccessLayer.DatabasesManager.Interfaces;

using Domain.KernelInterfaces;


namespace SentenceAPI.Features.UserPhoto.Interfaces
{
    interface IUserPhotoServiceFactory : IServiceFactory
    {
        IUserPhotoService GetService(IFactoriesManager factoriesManager, IDatabaseManager databasesManager);
    }
}
