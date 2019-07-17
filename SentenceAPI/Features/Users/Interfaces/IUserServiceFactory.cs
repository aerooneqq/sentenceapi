using SentenceAPI.Features.Users.Models;

namespace SentenceAPI.Features.Users.Interfaces
{
    public interface IUserServiceFactory
    {
        IUserService<UserInfo> GetService();
    }
}
