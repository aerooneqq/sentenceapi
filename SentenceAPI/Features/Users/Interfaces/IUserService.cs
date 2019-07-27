using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SentenceAPI.KernelInterfaces;
using SentenceAPI.KernelModels;

namespace SentenceAPI.Features.Users.Interfaces
{
    public interface IUserService<T> : IService where T : UniqueEntity
    {
        Task<T> Get(string email, string password);
        Task<T> Get(long id);
        Task<T> Get(string token);
        Task<long> CreateNewUser(string email, string password);
        void Delete(long id);
        Task Update(T user);
        Task<IEnumerable<T>> FindUsersWithLogin(string login);
    }
}
