using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentenceAPI.Features.Users.Interfaces
{
    public interface IUserService<T> where T : new()
    {
        T Get(string login, string password);
        void Insert(T user);
        void Delete(long id);
        void Update(T user);
    }
}
