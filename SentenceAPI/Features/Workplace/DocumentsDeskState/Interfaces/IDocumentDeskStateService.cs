using SentenceAPI.Features.Workplace.DocumentsDeskState.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentenceAPI.Features.Workplace.DocumentsDeskState.Interfaces
{
    public interface IDocumentDeskStateService
    {
        Task<DocumentDeskState> GetDeskStateAsync(long userID);
        Task<DocumentDeskState> GetDeskStateAsync(string token);

        Task UpdateDeskStateAsync(DocumentDeskState documentDeskState);

        Task CreateDeskStateAsync(long userID);

        Task DeleteDeskStateAsync(long userID);
    }
}
