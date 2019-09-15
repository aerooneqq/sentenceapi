using SentenceAPI.Features.Workplace.DocumentsStorage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentenceAPI.Features.Workplace.DocumentsStorage.Interfaces
{
    interface IFolderService
    {
        Task<IEnumerable<DocumentFolder>> GetFolders(long userID, long parentFolderID);
        Task<DocumentFolder> GetFolderData(long folderID);

        Task CreateFolder(long userID, long parentFolderID, string folderName);
        Task DeleteFolder(long folderID);
        Task RenameFolder(long folderID, string newFolderName);
    }
}
