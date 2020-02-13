using System.Collections.Generic;
using System.Threading.Tasks;

using Domain.KernelInterfaces;
using Domain.UserActivity;

using MongoDB.Bson;


namespace Application.UserActivity.Interfaces
{
    public interface IUserActivityService : IService
    {
        Task<Domain.UserActivity.UserActivity> GetUserActivityAsync(ObjectId userID);
        Task UpdateActivityAsync(Domain.UserActivity.UserActivity userActivity, IEnumerable<string> properties);
        Task AddSingleActivityAsync(ObjectId userID, SingleUserActivity singleUserActivity);
        Task<IEnumerable<SingleUserActivity>> GetUserSingleActivitiesAsync(ObjectId userID);
    }
}
