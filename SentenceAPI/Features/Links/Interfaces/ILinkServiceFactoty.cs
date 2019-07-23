using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SentenceAPI.KernelInterfaces;

namespace SentenceAPI.Features.Links.Interfaces
{
    public interface ILinkServiceFactoty : IFactory
    {
        ILinkService GetService();
    }
}
