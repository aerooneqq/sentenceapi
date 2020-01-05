using System.Threading.Tasks;

using DocumentsAPI.Features.DocumentStructure.Models;
using DocumentsAPI.Models.DocumentStructureModels;

using MongoDB.Bson;

using SharedLibrary.KernelInterfaces;


namespace DocumentsAPI.Features.DocumentStructure.Interfaces
{
    public interface IDocumentStructureService : IService
    {
        Task<DocumentStructureModel> GetDocumentStructureAsync(ObjectId documentID);
        Task UpdateStructureAsync(DocumentStructureModel documentStructure, ItemUpdateDto itemUpdateDto);
    }
}