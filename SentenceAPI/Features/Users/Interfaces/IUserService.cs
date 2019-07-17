using SentenceAPI.KernelInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentenceAPI.Features.Users.Interfaces
{
    public interface IUserService<T> : IService
    {
        T Get(string email, string password);
        void Insert(T user);
        void Delete(long id);
        void Update(T user);
    }
}
