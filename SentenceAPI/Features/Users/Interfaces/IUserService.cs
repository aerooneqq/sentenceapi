using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccessLayer.KernelModels;

using SharedLibrary.KernelInterfaces;

namespace SentenceAPI.Features.Users.Interfaces
{
    public interface IUserService<T> : IService where T : UniqueEntity
    {
        Task<T> GetAsync(string email, string password);
        Task<T> GetAsync(long id);
        Task<T> GetAsync(string token);

        Task<long> CreateNewUserAsync(string email, string password);

        Task DeleteAsync(long id);

        Task UpdateAsync(T user);
        Task UpdateAsync(T user, IEnumerable<string> properties);

        Task<IEnumerable<T>> FindUsersWithLoginAsync(string login);
        Task<bool> DoesUserExistAsync(string email);
    }
}
