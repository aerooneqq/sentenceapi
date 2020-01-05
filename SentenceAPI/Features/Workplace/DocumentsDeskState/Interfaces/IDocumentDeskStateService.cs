using MongoDB.Bson;
using SentenceAPI.Features.Workplace.DocumentsDeskState.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
