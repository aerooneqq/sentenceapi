using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using SentenceAPI.Features.Users.Models;
using SentenceAPI.KernelInterfaces;
using SentenceAPI.KernelModels;

namespace SentenceAPI.Features.Authentication.Interfaces
{
    public interface ITokenService : IService
    {
        bool CheckToken();
        string CreateEncodedToken(UserInfo user);
    }
}
