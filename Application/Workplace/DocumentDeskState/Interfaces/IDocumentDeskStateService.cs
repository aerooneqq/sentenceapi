using MongoDB.Bson;

using System.Threading.Tasks;

using Domain.Workplace.DocumentsDeskState;


namespace SentenceAPI.Features.Workplace.DocumentsDeskState.Interfaces
{
    public interface IDocumentDeskStateService
    {
        Task<DocumentDeskState> GetDeskStateAsync(ObjectId userID);
        Task<DocumentDeskState> GetDeskStateAsync(string token);

        Task UpdateDeskStateAsync(DocumentDeskState documentDeskState);

        Task CreateDeskStateAsync(ObjectId userID);

        Task DeleteDeskStateAsync(ObjectId userID);
    }
}
