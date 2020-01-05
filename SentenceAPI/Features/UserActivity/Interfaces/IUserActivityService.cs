using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using SentenceAPI.Features.UserActivity.Models;

using SharedLibrary.KernelInterfaces;

namespace SentenceAPI.Features.UserActivity.Interfaces
{
    public interface IUserActivityService : IService
    {
        Task<Models.UserActivity> GetUserActivityAsync(ObjectId userID);
        Task UpdateActivityAsync(Models.UserActivity userActivity, IEnumerable<string> properties);
        Task AddSingleActivityAsync(ObjectId userID, SingleUserActivity singleUserActivity);
        Task<IEnumerable<SingleUserActivity>> GetUserSingleActivitiesAsync(ObjectId userID);
    }
}
