using System.Threading.Tasks;

using MongoDB.Bson;

using SharedLibrary.KernelInterfaces;


namespace DocumentsAPI.Features.FileToDocument.Interfaces
{
    public interface IFileToDocumentService : IService
    {
        Task<ObjectId> GetDocumentIDAsync(ObjectId fileID);
        Task AssociateFileWithDocumentAsync(ObjectId fileID, ObjectId documentID);
        Task<Models.FileToDocument> GetFileToDocumentModelAsync(ObjectId fileID);
    }
}