using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentenceAPI.Features.Authentication.Interfaces
{
    interface ITokenServiceFactory
    {
        ITokenService GetService();
    }
}
