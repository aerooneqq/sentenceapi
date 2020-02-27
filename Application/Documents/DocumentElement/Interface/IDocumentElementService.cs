using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Documents.DocumentElement.Models;
using Domain.DocumentElements.Dto;
using Domain.KernelInterfaces;

using MongoDB.Bson;


namespace Application.Documents.DocumentElement.Interface
{
    public interface IDocumentElementService : IService
    {
        Task<IEnumerable<DocumentElementDto>> GetDocumentElementsAsync(ObjectId parentItemID, ObjectId userID);
        Task RenameDocumentElementAsync(DocumentElementRenameDto renameDto);
        Task UpdateContentInBranchNodeAsync(DocumentElementContentUpdateDto updateDto);
        Task DeleteDocumentElementAsync(DocumentElementDeleteDto deleteDto);
        Task<ObjectId> CreateNewDocumentElementAsync(DocumentElementCreateDto createDto);
    }
}