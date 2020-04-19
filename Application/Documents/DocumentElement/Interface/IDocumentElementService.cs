using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Documents.DocumentElement.Models;
using Domain.KernelInterfaces;

using MongoDB.Bson;


namespace Application.Documents.DocumentElement.Interface
{
    public interface IDocumentElementService : IService
    {
        Task<IEnumerable<DocumentElementDto>> GetDocumentElementsAsync(ObjectId documentID, 
                                                                       ObjectId parentItemID, ObjectId userID);
        Task RenameDocumentElementAsync(DocumentElementRenameDto renameDto);
        Task DeleteDocumentElementAsync(ObjectId elementID);
        Task<ObjectId> CreateNewDocumentElementAsync(DocumentElementCreateDto createDto);
        Task ChangeSelectedBranch(ObjectId documentElementID, ObjectId branchID);
        Task ChangeSelectedBranchNode(ObjectId documentElementID, ObjectId branchNodeID);
        Task<DocumentElementDto> GetDocumentElementAsync(ObjectId elementID, ObjectId userID);
    }
}