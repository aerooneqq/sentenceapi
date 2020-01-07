using System.Threading.Tasks;

using MongoDB.Bson;

using SentenceAPI.Features.Users.Models;
using SentenceAPI.Features.Workplace.DocumentsStorage.Models;

using SharedLibrary.KernelInterfaces;


namespace SentenceAPI.Features.Workplace.DocumentsStorage.Interfaces
{
    public interface IUserMainFoldersService : IService
    {
        Task<UserMainFolders> GetUserMainFoldersAsync(ObjectId userID);
        Task UpdateUserMainFolders(UserMainFolders userMainFolders);
        Task<ObjectId> CreateNewUserMainFolders(ObjectId userID);
    }
}