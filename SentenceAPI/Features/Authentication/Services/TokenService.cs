using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.IdentityModel.Tokens;

using SentenceAPI.Features.Authentication.Interfaces;
using SentenceAPI.Features.Authentication.Models;
using SentenceAPI.Features.Users.Models;
using SentenceAPI.Databases.MongoDB.Interfaces;
using SentenceAPI.Features.FactoriesManager.Models;

namespace SentenceAPI.Features.Authentication.Services
{
    public class TokenService : ITokenService
    {
        #region Services
        private IMongoDBService<JwtToken> mongoDBService;
        #endregion

        #region Builders
        private IMongoDBServiceBuilder<JwtToken> mongoDBServiceBuilder;
        #endregion

        #region Factories
        private FactoriesManager.FactoriesManager factoriesManager = FactoriesManager.FactoriesManager.Instance;
        private IMongoDBServiceFactory mongoDBServiceFactory;
        #endregion

        #region Constructors
        public TokenService()
        {
            mongoDBServiceFactory = factoriesManager[typeof(IMongoDBServiceFactory)].Factory
                as IMongoDBServiceFactory;

            mongoDBServiceBuilder = mongoDBServiceFactory.GetBuilder(mongoDBServiceFactory.GetService<JwtToken>());
            mongoDBService = mongoDBServiceBuilder.AddConfigurationFile("database_config.json")
                .SetConnectionString()
                .SetDatabaseName("SentenceDatabase")
                .SetCollectionName()
                .Build();
        }
        #endregion

        public bool CheckToken(string encodedToken)
        {
            throw new NotImplementedException();
        }

        public async Task InsertTokenInDB(JwtToken token)
        {
            await mongoDBService.Connect();
            await mongoDBService.Insert(token);
        }

        /// <summary>
        /// Creates the Jwt security token
        /// </summary>
        /// <returns>
        /// The encoded token (string) and the JwtSecurityToken object
        /// </returns>
        public (string encodedToken, JwtSecurityToken securityToken) CreateEncodedToken(UserInfo user)
        {
            var now = DateTime.UtcNow;
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
            return (encodedToken, jwtToken);
        }

        public LifetimeValidator GetLifeTimeValidationDel()
        {
            return (notBefore, exp, token, parameters) =>
            {
                var now = DateTime.UtcNow;

                if (now < notBefore)
                    return false;
                if (now > exp)
                    return false;

                return true;
            };
        }

        private ClaimsIdentity GetUserIdentity(UserInfo user)
        {
            ClaimsIdentity userIdentity = new ClaimsIdentity();

            userIdentity.AddClaim(new Claim("Email", user.Email));
            userIdentity.AddClaim(new Claim("Login", user.Login));
            userIdentity.AddClaim(new Claim("ID", user.ID.ToString()));

            return userIdentity;
        }

        public string GetTokenClaim(string token, string claimType)
        {
            JwtSecurityToken jwtSecurityToken = new JwtSecurityTokenHandler().ReadToken(token) as JwtSecurityToken;
            return jwtSecurityToken.Claims.ToList().Find(c => c.Type == claimType).Value;
        }
    }
}
