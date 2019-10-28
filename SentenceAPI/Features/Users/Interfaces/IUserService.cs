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
        Task<T> Get(string email, string password);
        Task<T> Get(long id);
        Task<T> Get(string token);

        Task<long> CreateNewUser(string email, string password);

        Task Delete(long id);

        Task Update(T user);
        Task Update(T user, IEnumerable<string> properties);

        Task<IEnumerable<T>> FindUsersWithLogin(string login);
        Task<bool> DoesUserExist(string email);
    }
}
