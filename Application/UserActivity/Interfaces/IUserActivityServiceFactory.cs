using DataAccessLayer.DatabasesManager.Interfaces;
using Domain.KernelInterfaces;

namespace Application.UserActivity.Interfaces
{
    public interface IUserActivityServiceFactory : IServiceFactory
    {
        IUserActivityService GetService(IDatabaseManager databasesManager);
    }
}
