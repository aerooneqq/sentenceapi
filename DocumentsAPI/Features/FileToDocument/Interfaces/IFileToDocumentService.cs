using System.Threading.Tasks;
using SharedLibrary.KernelInterfaces;

namespace DocumentsAPI.Features.FileToDocument.Interfaces
{
    public interface IFileToDocumentService : IService
    {
        Task<long> GetDocumentIDAsync(long fileID);
        Task AssociateFileWithDocumentAsync(long fileID, long documentID);
    }
}