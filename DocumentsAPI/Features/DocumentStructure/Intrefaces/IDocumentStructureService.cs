using System.Threading.Tasks;

using DocumentsAPI.Features.DocumentStructure.Models;

using Domain.DocumentStructureModels;
using Domain.KernelInterfaces;

using MongoDB.Bson;


namespace DocumentsAPI.Features.DocumentStructure.Interfaces
{
    public interface IDocumentStructureService : IService
    {
        Task<DocumentStructureModel> GetDocumentStructureAsync(ObjectId documentID);
        Task UpdateStructureAsync(DocumentStructureModel documentStructure, ItemUpdateDto itemUpdateDto);
        Task<ObjectId> CreateNewDocumentStructure(ObjectId documentID);
    }
}