using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using SentenceAPI.Features.Authentication.Interfaces;
using SentenceAPI.Features.Authentication.Models;
using SentenceAPI.Features.Users.Models;
using Microsoft.IdentityModel.Tokens;

namespace SentenceAPI.Features.Authentication.Services
{
    public class TokenService : ITokenService
    {
        public bool CheckToken()
        {
            throw new NotImplementedException();
        }

        public string CreateEncodedToken(UserInfo user)
        {
            var now = DateTime.Now;
            var jwtToken = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    expires: now.Add(TimeSpan.FromSeconds(AuthOptions.SecondsLifeTime)),
                    claims: GetUserIdentity(user).Claims,
                    notBefore: now,
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(),
                        SecurityAlgorithms.HmacSha256)
                );  

            var encodedToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            return encodedToken;
        }

        private ClaimsIdentity GetUserIdentity(UserInfo user)
        {
            ClaimsIdentity userIdentity = new ClaimsIdentity();

            userIdentity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
            userIdentity.AddClaim(new Claim("Login", user.Login));

            return userIdentity;
        }
    }
}
