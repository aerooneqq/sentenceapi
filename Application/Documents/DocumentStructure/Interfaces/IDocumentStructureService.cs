using System.Threading.Tasks;

using Application.Documents.DocumentStructure.Models;

using Domain.DocumentStructureModels;
using Domain.KernelInterfaces;

using MongoDB.Bson;


namespace Application.Documents.DocumentStructure.Interfaces
{
    public interface IDocumentStructureService : IService
    {
        Task<DocumentStructureModel> GetDocumentStructureAsync(ObjectId documentID);
        Task UpdateStructureAsync(DocumentStructureModel documentStructure, ItemUpdateDto itemUpdateDto);
        Task<ObjectId> CreateNewDocumentStructure(ObjectId documentID, string documentName, ObjectId userID);
    }
}