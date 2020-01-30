using System.Threading.Tasks;

using Domain.KernelInterfaces;

using SentenceAPI.Features.UserFeed.Models;

using MongoDB.Bson;


namespace SentenceAPI.Features.UserFeed.Interfaces
{
    public interface IUserFeedService : IService
    {
        Task<UsersFeedDto> GetUserFeedAsync(string token);
        Task<UsersFeedDto> GetUserFeedAsync(ObjectId userID);

        Task InsertUserPostAsync(Domain.UserFeed.UserFeed userFeed);
        Task InsertUserPostAsync(string token, string message);
    }
}
