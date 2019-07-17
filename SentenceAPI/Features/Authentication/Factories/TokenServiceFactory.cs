using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SentenceAPI.Features.Authentication.Interfaces;
using SentenceAPI.Features.Authentication.Services;

namespace SentenceAPI.Features.Authentication.Factories
{
    public class TokenServiceFactory : ITokenServiceFactory
    {
        public ITokenService GetService()
        {
            return new TokenService();
        }
    }
}
