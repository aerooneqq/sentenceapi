using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

using SentenceAPI.Features.Authentication.Interfaces;

namespace SentenceAPI.Features.Authentication.Services
{
    public class TokenService : ITokenService
    {
        public bool CheckToken()
        {
            throw new NotImplementedException();
        }

        public JwtSecurityToken CreateToken()
        {
            throw new NotImplementedException();
        }
    }
}
