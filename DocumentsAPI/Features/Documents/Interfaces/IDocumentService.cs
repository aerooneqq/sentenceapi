using System.Threading.Tasks;

using DocumentsAPI.Models.Document;

using MongoDB.Bson;

using SharedLibrary.KernelInterfaces;


namespace DocumentsAPI.Features.Documents.Interfaces
{
    public interface IDocumentService : IService
    {
        Task<ObjectId> CreateEmptyDocument(ObjectId userID, string name, DocumentType documentType);
    }
}