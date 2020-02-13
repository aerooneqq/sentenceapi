using System.Threading.Tasks;

using Domain.Users;


namespace SentenceAPI.Features.Links.Interfaces
{
    public interface ILinkService
    {
        Task<string> CreateVerificationLinkAsync(UserInfo user);
        Task<bool?> ActivateLinkAsync(string link);
    }
}