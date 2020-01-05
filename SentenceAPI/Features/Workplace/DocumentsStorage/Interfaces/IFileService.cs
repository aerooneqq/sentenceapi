using MongoDB.Bson;
using SentenceAPI.Features.Workplace.DocumentsStorage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentenceAPI.Features.Workplace.DocumentsStorage.Interfaces
{
    interface IFileService
    {
        Task<IEnumerable<DocumentFile>> GetFilesAsync(ObjectId userID, ObjectId parentFolderID);
        Task CreateNewFileAsync(ObjectId userID, ObjectId parentFileID, string fileName);
        Task DeleteFileAsync(ObjectId fileID);
        Task UpdateAsync(DocumentFile documentFile);
        Task<IEnumerable<DocumentFile>> GetFilesAsync(ObjectId userID, string searchQuery);
        Task<DocumentFile> GetFileAsync(ObjectId fileID);
        Task RenameFileAsync(ObjectId fileID, string newFileName);
    }
}
