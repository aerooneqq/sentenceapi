using System.Threading.Tasks;

using Domain.KernelInterfaces;
using Domain.Workplace.DocumentsStorage;

using MongoDB.Bson;


namespace SentenceAPI.Features.Workplace.DocumentsStorage.Interfaces
{
    public interface IUserMainFoldersService : IService
    {
        Task<UserMainFolders> GetUserMainFoldersAsync(ObjectId userID);
        Task UpdateUserMainFolders(UserMainFolders userMainFolders);
        Task<ObjectId> CreateNewUserMainFolders(ObjectId userID);
    }
}