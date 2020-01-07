using MongoDB.Bson;
using SentenceAPI.Features.Workplace.DocumentsStorage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentenceAPI.Features.Workplace.DocumentsStorage.Interfaces
{
    interface IFolderService
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
