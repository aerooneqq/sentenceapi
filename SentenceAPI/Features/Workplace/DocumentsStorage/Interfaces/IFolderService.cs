using MongoDB.Bson;

using System.Collections.Generic;
using System.Threading.Tasks;

using Domain.Workplace.DocumentsStorage;


namespace SentenceAPI.Features.Workplace.DocumentsStorage.Interfaces
{
    internal interface IFolderService
    {
        Task<IEnumerable<DocumentFolder>> GetFolders(ObjectId userID, ObjectId parentFolderID);
        Task<DocumentFolder> GetFolderData(ObjectId folderID);

        Task<ObjectId> CreateFolder(ObjectId userID, ObjectId parentFolderID, string folderName);
        Task DeleteFolder(ObjectId folderID);
        Task RenameFolder(ObjectId folderID, string newFolderName);
        Task<IEnumerable<DocumentFolder>> GetFolders(ObjectId userID, string searchQuery);
        Task Update(DocumentFolder folder);
    }
}
