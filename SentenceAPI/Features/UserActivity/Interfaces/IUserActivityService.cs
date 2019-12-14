using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SentenceAPI.Features.UserActivity.Models;

using SharedLibrary.KernelInterfaces;

namespace SentenceAPI.Features.UserActivity.Interfaces
{
    public interface IUserActivityService : IService
    {
        Task<Models.UserActivity> GetUserActivityAsync(long userID);
        Task UpdateActivityAsync(Models.UserActivity userActivity, IEnumerable<string> properties);
        Task AddSingleActivityAsync(long userID, SingleUserActivity singleUserActivity);
        Task<IEnumerable<SingleUserActivity>> GetUserSingleActivitiesAsync(long userID);
    }
}
