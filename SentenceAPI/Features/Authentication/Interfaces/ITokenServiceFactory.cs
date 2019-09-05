using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SentenceAPI.KernelInterfaces;

namespace SentenceAPI.Features.Authentication.Interfaces
{
    public interface ITokenServiceFactory : IServiceFactory
    {
        ITokenService GetService();
    }
}
