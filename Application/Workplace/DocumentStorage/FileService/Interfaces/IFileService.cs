﻿using MongoDB.Bson;

using System.Collections.Generic;
using System.Threading.Tasks;

using Domain.Workplace.DocumentsStorage;


namespace SentenceAPI.Features.Workplace.DocumentsStorage.Interfaces
{
    internal interface IFileService
    {
        Task<IEnumerable<DocumentFile>> GetFilesAsync(ObjectId userID, ObjectId parentFolderID);
        Task<ObjectId> CreateNewFileAsync(ObjectId userID, ObjectId parentFileID, string fileName);
        Task DeleteFileAsync(ObjectId fileID);
        Task UpdateAsync(DocumentFile documentFile);
        Task<IEnumerable<DocumentFile>> GetFilesAsync(ObjectId userID, string searchQuery);
        Task<DocumentFile> GetFileAsync(ObjectId fileID);
        Task RenameFileAsync(ObjectId fileID, string newFileName);
    }
}
