using System.Collections.Generic;
using System.Threading.Tasks;

using Domain.DocumentElements.Dto;
using Domain.KernelInterfaces;

using MongoDB.Bson;


namespace Application.Documents.DocumentElement.Interface
{
    public interface IDocumentElementService : IService
    {
        Task<IEnumerable<DocumentElementDto>> GetDocumentElements(ObjectId parentItemID);
    }
}