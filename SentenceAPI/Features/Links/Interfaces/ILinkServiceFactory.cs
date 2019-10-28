using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SharedLibrary.KernelInterfaces;

namespace SentenceAPI.Features.Links.Interfaces
{
    public interface ILinkServiceFactory : IServiceFactory
    {
        ILinkService GetService();
    }
}
