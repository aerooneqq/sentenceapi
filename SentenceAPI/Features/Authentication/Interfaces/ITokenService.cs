using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.IdentityModel.Tokens;

using SentenceAPI.Features.Authentication.Models;
using SentenceAPI.Features.Users.Models;
using SharedLibrary.KernelInterfaces;

namespace SentenceAPI.Features.Authentication.Interfaces
{   
    public interface ITokenService : IService
    {
        bool CheckToken(string encodedToken);
        (string encodedToken, JwtSecurityToken securityToken) CreateEncodedToken(UserInfo user);
        Task InsertTokenInDBAsync(JwtToken token);
        string GetTokenClaim(string token, string claimType);
    }
}
