using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

using SentenceAPI.KernelModels;

namespace SentenceAPI.Features.Authentication.Interfaces
{
    public interface ITokenService
    {
        bool CheckToken();
        JwtSecurityToken CreateToken();
    }
}
