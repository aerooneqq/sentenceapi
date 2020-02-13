using DataAccessLayer.DatabasesManager.Interfaces;
using Domain.KernelInterfaces;

namespace SentenceAPI.Features.UserActivity.Interfaces
{
    public interface IUserActivityServiceFactory : IServiceFactory
    {
        IUserActivityService GetService(IDatabaseManager databasesManager);
    }
}
