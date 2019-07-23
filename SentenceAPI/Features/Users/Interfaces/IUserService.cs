using SentenceAPI.KernelInterfaces;
using SentenceAPI.KernelModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentenceAPI.Features.Users.Interfaces
{
    public interface IUserService<T> : IService where T : UniqueEntity
    {
        Task<T> Get(string email, string password);
        Task<T> Get(long id);
        Task<long> CreateNewUser(string email, string password);
        void Delete(long id);
        void Update(T user);
    }
}
