using SentenceAPI.Features.Workplace.DocumentsStorage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentenceAPI.Features.Workplace.DocumentsStorage.Interfaces
{
    interface IFileService
    {
        Task<IEnumerable<DocumentFile>> GetFiles(long userID, long parentFolderID);
        Task CreateNewFile(long userID, long parentFileID, string fileName);
        Task DeleteFile(long fileID);
        Task Update(DocumentFile documentFile);
        Task<IEnumerable<DocumentFile>> GetFiles(long userID, string searchQuery);
        Task<DocumentFile> GetFile(long fileID);
        Task RenameFile(long fileID, string newFileName);
    }
}
