using System.Threading.Tasks;

using Application.UserFeed.Models;

using Domain.KernelInterfaces;

using MongoDB.Bson;


namespace Application.UserFeed.Interfaces
{
    public interface IUserFeedService : IService
    {
        Task<UsersFeedDto> GetUserFeedAsync(string token);
        Task<UsersFeedDto> GetUserFeedAsync(ObjectId userID);

        Task InsertUserPostAsync(Domain.UserFeed.UserFeed userFeed);
        Task InsertUserPostAsync(string token, string message);
    }
}
