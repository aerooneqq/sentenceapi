using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Documents.DocumentElement.Models;
using Domain.KernelInterfaces;

using MongoDB.Bson;


namespace Application.Documents.DocumentElement.Interface
{
    public interface IBranchNodeService : IService
    {
        Task<DocumentElementDto> CreateNewNodeAsync(ObjectId documentElementID, ObjectId branchID, 
                                                    ObjectId userID, string nodeName, string comment);

        Task<DocumentElementDto> UpdateNodePropertiesAsync(ObjectId documentElementID, ObjectId branchNodeID,
                                                           ObjectId userID, string newName, string newComment);

        Task<DocumentElementDto> DeleteNodeAsync(ObjectId documentElementID, ObjectId branchNodeID, ObjectId userID);
        Task<DocumentElementDto> UpdateContentAsync(DocumentElementContentUpdateDto dto);
    }
}