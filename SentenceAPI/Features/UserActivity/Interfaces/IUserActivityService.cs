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
        Task<Models.UserActivity> GetUserActivity(long userID);
        Task UpdateActivity(Models.UserActivity userActivity, IEnumerable<string> properties);
        Task AddSingleActivity(long userID, SingleUserActivity singleUserActivity);
        Task<IEnumerable<SingleUserActivity>> GetUserSingleActivities(long userID);
    }
}
