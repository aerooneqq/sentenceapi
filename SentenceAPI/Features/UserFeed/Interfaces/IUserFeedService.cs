using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SharedLibrary.KernelInterfaces;

using SentenceAPI.Features.UserFeed.Models;
using MongoDB.Bson;

namespace SentenceAPI.Features.UserFeed.Interfaces
{
    public interface IUserFeedService : IService
    {
        Task<UsersFeedDto> GetUserFeedAsync(string token);
        Task<UsersFeedDto> GetUserFeedAsync(ObjectId userID);

        Task InsertUserPostAsync(Models.UserFeed userFeed);
        Task InsertUserPostAsync(string token, string message);
    }
}
