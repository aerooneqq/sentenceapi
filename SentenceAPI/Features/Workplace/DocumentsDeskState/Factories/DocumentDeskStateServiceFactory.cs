using SentenceAPI.Features.Workplace.DocumentsDeskState.Interfaces;
using SentenceAPI.Features.Workplace.DocumentsDeskState.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentenceAPI.Features.Workplace.DocumentsDeskState.Factories
{
    public class DocumentDeskStateServiceFactory : IDocumentDeskStateServiceFactory
    {
        public IDocumentDeskStateService GetService()
        {
            return new DocumentDeskStateService();
        }
    }
}
