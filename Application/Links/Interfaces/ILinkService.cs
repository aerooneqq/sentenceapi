using System.Threading.Tasks;
using Domain.Links;
using Domain.Users;
using MongoDB.Bson;


namespace Application.Links.Interfaces
{
    public interface ILinkService
    {
        Task<string> CreateVerificationLinkAsync(UserInfo user);
        Task<bool?> ActivateLinkAsync(string link);
        Task<WordDownloadLink> CreateWordDownloadLink(ObjectId documentID, ObjectId userID);
        Task<WordDownloadLink> GetUnusedDownloadLink(ObjectId linkID);
        Task MarkWordLinkAsUsed(ObjectId linkID);
    }
}