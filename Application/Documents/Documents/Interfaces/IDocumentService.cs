using System.Threading.Tasks;

using Domain.KernelInterfaces;
using Domain.Models.Document;

using MongoDB.Bson;


namespace Application.Documents.Documents.Interfaces
{
    public interface IDocumentService : IService
    {
        Task<ObjectId> CreateEmptyDocument(ObjectId userID, string name, DocumentType documentType);
        Task<Document> GetDocumentByID(ObjectId documentID);
        Task DeleteDocumentByID(ObjectId documentID);
    }
}