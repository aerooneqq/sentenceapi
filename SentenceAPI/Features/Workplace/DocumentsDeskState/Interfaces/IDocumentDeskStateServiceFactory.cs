using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SharedLibrary.KernelInterfaces;

namespace SentenceAPI.Features.Workplace.DocumentsDeskState.Interfaces
{
    public interface IDocumentDeskStateServiceFactory : IServiceFactory
    {
        IDocumentDeskStateService GetService();
    }
}
