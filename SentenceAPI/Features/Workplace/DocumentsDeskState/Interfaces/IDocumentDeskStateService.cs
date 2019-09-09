using SentenceAPI.Features.Workplace.DocumentsDeskState.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentenceAPI.Features.Workplace.DocumentsDeskState.Interfaces
{
    public interface IDocumentDeskStateService
    {
        Task<DocumentDeskState> GetDeskState(long userID);
        Task<DocumentDeskState> GetDeskState(string token);

        Task UpdateDeskState(DocumentDeskState documentDeskState);

        Task CreateDeskState(long userID);

        Task DeleteDeskState(long userID);
    }
}
