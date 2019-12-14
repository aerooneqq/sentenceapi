using SentenceAPI.Features.Workplace.DocumentsStorage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentenceAPI.Features.Workplace.DocumentsStorage.Interfaces
{
    interface IFileService
    {
        Task<IEnumerable<DocumentFile>> GetFilesAsync(long userID, long parentFolderID);
        Task CreateNewFileAsync(long userID, long parentFileID, string fileName);
        Task DeleteFileAsync(long fileID);
        Task UpdateAsync(DocumentFile documentFile);
        Task<IEnumerable<DocumentFile>> GetFilesAsync(long userID, string searchQuery);
        Task<DocumentFile> GetFileAsync(long fileID);
        Task RenameFileAsync(long fileID, string newFileName);
    }
}
