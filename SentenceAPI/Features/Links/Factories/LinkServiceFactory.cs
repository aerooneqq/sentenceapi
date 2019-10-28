using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SentenceAPI.Features.Links.Interfaces;
using SentenceAPI.Features.Links.Services;
using SharedLibrary.KernelInterfaces;

namespace SentenceAPI.Features.Links.Factories
{
    public class LinkServiceFactory : ILinkServiceFactory
    {
        public ILinkService GetService()
        {
            return new LinkService();
        }
    }
}
