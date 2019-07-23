using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SentenceAPI.Features.Links.Interfaces;
using SentenceAPI.Features.Links.Services;

namespace SentenceAPI.Features.Links.Factories
{
    public class LinkServiceFactory : ILinkServiceFactoty
    {
        public ILinkService GetService()
        {
            return new LinkService();
        }
    }
}
