using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.IdentityModel.Tokens;

using SentenceAPI.Features.Authentication.Interfaces;
using SentenceAPI.Features.Authentication.Models;
using SentenceAPI.Features.Users.Models;

using DataAccessLayer.CommonInterfaces;
using DataAccessLayer.Configuration.Interfaces;
using DataAccessLayer.Configuration;
using DataAccessLayer.DatabasesManager.Interfaces;

using SharedLibrary.Loggers.Models;
using SharedLibrary.Loggers.Interfaces;
using SharedLibrary.Loggers.Configuration;
using DataAccessLayer.Exceptions;

namespace SentenceAPI.Features.Authentication.Services
{
    public class TokenService : ITokenService
    {
        #region Static properties
        private static readonly string databaseConfigFile = "./configs/mongo_database_config.json";
        #endregion

        #region Databases
        private IDatabaseService<JwtToken> mongoDBService;
        private IConfigurationBuilder configurationBuilder;
        #endregion


        #region Services
        private ILogger<ApplicationError> exceptionLogger;
        #endregion

        private readonly LogConfiguration logConfiguration;


        #region Constructors
        public TokenService(IDatabaseManager databaseManager)
        {
            databaseManager.MongoDBFactory.GetDatabase<JwtToken>().TryGetTarget(out mongoDBService);
            configurationBuilder = new MongoConfigurationBuilder(mongoDBService.Configuration);

            configurationBuilder.SetConfigurationFilePath(databaseConfigFile).SetUserName().SetPassword()
                                .SetAuthMechanism().SetDatabaseName().SetServerName().SetConnectionString();

            logConfiguration = new LogConfiguration(GetType());
        }
        #endregion


        public bool CheckToken(string encodedToken)
        {
            throw new NotImplementedException();
        }

        public async Task InsertTokenInDBAsync(JwtToken token)
        {
            try
            {
                await mongoDBService.Connect().ConfigureAwait(false);
                await mongoDBService.Insert(token).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("Error occured while working with token", ex);
            }
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

        private ClaimsIdentity GetUserIdentity(UserInfo user)
        {
            ClaimsIdentity userIdentity = new ClaimsIdentity();

            userIdentity.AddClaim(new Claim("Email", user.Email));
            userIdentity.AddClaim(new Claim("Login", user.Login ?? string.Empty));
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
