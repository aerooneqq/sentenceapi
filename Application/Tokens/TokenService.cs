using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using Application.Tokens.Interfaces;

using DataAccessLayer.CommonInterfaces;
using DataAccessLayer.Configuration.Interfaces;
using DataAccessLayer.Configuration;
using DataAccessLayer.DatabasesManager.Interfaces;
using DataAccessLayer.Exceptions;

using SharedLibrary.Loggers.Interfaces;

using Domain.Authentication;
using Domain.Logs;
using Domain.Logs.Configuration;
using Domain.Users;

using Microsoft.IdentityModel.Tokens;

using SharedLibrary.FactoriesManager.Interfaces;


namespace Application.Tokens
{
    public class TokenService : ITokenService
    {
        #region Constants
        private const string DATABASE_CONFIG_FILE = "./configs/mongo_database_config.json";
        #endregion

        #region Databases
        private readonly IDatabaseService<JwtToken> mongoDBService;
        #endregion


        #region Services
        private ILogger<ApplicationError> exceptionLogger;
        #endregion

        private readonly LogConfiguration logConfiguration;


        #region Constructors
        public TokenService(IDatabaseManager databaseManager, IFactoriesManager factoriesManager)
        {
            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);
            
            databaseManager.MongoDBFactory.GetDatabase<JwtToken>().TryGetTarget(out mongoDBService);
            IConfigurationBuilder configurationBuilder = new MongoConfigurationBuilder(mongoDBService.Configuration);

            configurationBuilder.SetConfigurationFilePath(DATABASE_CONFIG_FILE).SetUserName().SetPassword()
                                .SetAuthMechanism().SetDatabaseName().SetServerName().SetConnectionString();

            logConfiguration = new LogConfiguration(GetType());
        }
        #endregion


        /// <summary>
        /// Checks if token is of valid format
        /// </summary>
        public bool CheckToken(string encodedToken)
        {
            try 
            {
                new JwtSecurityToken(encodedToken);
                return true;
            }
            catch (Exception) 
            {
                return false;
            }
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
            DateTime now = DateTime.UtcNow;
            JwtSecurityToken jwtToken = new JwtSecurityToken(
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

        /// <summary>
        /// Decodes the token, assuming that the token is of a valid format
        /// </summary>
        public JwtSecurityToken DecodeToken(string token) 
        {
            return new JwtSecurityToken(token);
        }

        private static ClaimsIdentity GetUserIdentity(UserInfo user)
        {
            ClaimsIdentity userIdentity = new ClaimsIdentity();

            userIdentity.AddClaim(new Claim("Email", user.Email));
            userIdentity.AddClaim(new Claim("Login", user.Login ?? string.Empty));
            userIdentity.AddClaim(new Claim("ID", user.ID.ToString()));

            foreach (Role role in user.Roles)
            {
                userIdentity.AddClaim(new Claim(role.ToString(), role.ToString()));
            }

            return userIdentity;
        }

        /// <summary>
        /// Gets the token claim. If token is null then returns null.
        /// If there is no claim with such a name returns null 
        /// </summary>
        public string GetTokenClaim(string token, string claimType)
        {
            JwtSecurityToken jwtSecurityToken = new JwtSecurityTokenHandler().ReadToken(token) as JwtSecurityToken;

            Claim claim = jwtSecurityToken?.Claims.FirstOrDefault(c => c.Type == claimType);
            return claim?.Value;
        }
    }
}
