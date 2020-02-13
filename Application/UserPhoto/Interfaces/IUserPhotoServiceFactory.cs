using SharedLibrary.FactoriesManager.Interfaces;

using DataAccessLayer.DatabasesManager.Interfaces;

using Domain.KernelInterfaces;


namespace Application.UserPhoto.Interfaces
{
    public interface IUserPhotoServiceFactory : IServiceFactory
    {
        IUserPhotoService GetService(IFactoriesManager factoriesManager, IDatabaseManager databasesManager);
    }
}
