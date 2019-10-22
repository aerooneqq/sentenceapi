using SentenceAPI.Features.UserFeed.Models;
using SentenceAPI.KernelInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentenceAPI.Features.UserFeed.Interfaces
{
    public interface IUserFeedService : IService
    {
        Task<UsersFeedDto> GetUserFeed(string token);
        Task<UsersFeedDto> GetUserFeed(long userID);

        Task InsertUserPost(Models.UserFeed userFeed);
        Task InsertUserPost(string token, string message);
    }
}
