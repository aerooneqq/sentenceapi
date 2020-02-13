using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.KernelInterfaces;
using Domain.KernelModels;
using MongoDB.Bson;


namespace SentenceAPI.Features.Users.Interfaces
{
    public interface IUserService<T> : IService where T : UniqueEntity
    {
        Task<T> GetAsync(string email, string password);
        Task<T> GetAsync(ObjectId id);
        Task<T> GetAsync(string token);

        Task<ObjectId> CreateNewUserAsync(string email, string password);

        Task DeleteAsync(ObjectId id);

        Task UpdateAsync(T user);
        Task UpdateAsync(T user, IEnumerable<string> properties);

        Task<IEnumerable<T>> FindUsersWithLoginAsync(string login);
        Task<bool> DoesUserExistAsync(string email);
    }
}
