using System.Threading.Tasks;

using Domain.KernelInterfaces;

using MongoDB.Bson;


namespace Application.Documents.FileToDocument.Interfaces
{
    public interface IFileToDocumentService : IService
    {
        Task<ObjectId> GetDocumentIDAsync(ObjectId fileID);
        Task AssociateFileWithDocumentAsync(ObjectId fileID, ObjectId documentID);
        Task<Models.FileToDocument> GetFileToDocumentModelAsync(ObjectId fileID);
    }
}