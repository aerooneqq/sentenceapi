using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;

using DataAccessLayer.CommonInterfaces;
using DataAccessLayer.DatabasesManager.Interfaces;
using DataAccessLayer.Exceptions;

using DocumentsAPI.Features.Authentication.Interfaces;
using DocumentsAPI.Features.Authentication.Models;

using Microsoft.IdentityModel.Tokens;

using MongoDB.Bson;

using SharedLibrary.Date.Interfaces;
using SharedLibrary.FactoriesManager.Interfaces;
using SharedLibrary.Loggers.Configuration;
using SharedLibrary.Loggers.Interfaces;
using SharedLibrary.Loggers.Models;


namespace DocumentsAPI.Features.Authentication.Services
{
    public class TokenService : ITokenService
    {
        #region Databases
        private readonly IDatabaseService<DocumentsJwtToken> database;
        private readonly ILogger<ApplicationError> exceptionLogger;
        private readonly IDateService dateService;
        #endregion

        private readonly LogConfiguration logConfiguration;


        public TokenService(IFactoriesManager factoriesManager, IDatabaseManager databaseManager)
        {
            databaseManager.MongoDBFactory.GetDatabase<DocumentsJwtToken>().TryGetTarget(out database);
            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);

            logConfiguration = new LogConfiguration(GetType());
        }


        public async Task InsertDocumentToken(DocumentsJwtToken token)
        {
            try
            {
                await database.Connect().ConfigureAwait(false);
                await database.Insert(token);
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("Error occured while working with JWT token", ex);
            }
        }

        /// <summary>
        /// Creates the Jwt security token
        /// </summary>
        /// <returns>
        /// The encoded token (string) and the JwtSecurityToken object
        /// </returns>
        public (string encodedToken, JwtSecurityToken securityToken) CreateEncodedToken(ObjectId userID, 
                                                                                        ObjectId parentTokenID,
                                                                                        ObjectId requestID)
        {
            var currDate = dateService.GetCurrentDate();
            var jwtToken = new JwtSecurityToken
                (
                    issuer: AuthOptions.Issuer,
                    audience: AuthOptions.Audienece,
                    expires: currDate.Add(TimeSpan.FromSeconds(AuthOptions.SecondsLifeTime)),
                    claims: GetClaims(userID, parentTokenID, requestID),
                    notBefore: dateService.GetCurrentDate(),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSecurityKey(),
                        SecurityAlgorithms.HmacSha256)
                );

            var encodedToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            return (encodedToken, jwtToken);
        }

        private IEnumerable<Claim> GetClaims(ObjectId userID, ObjectId parentTokenID, ObjectId requestID)
        {
            Claim userIDClaim = new Claim("UserID", userID.ToString());
            Claim parentTokenIDClaim = new Claim("ParentTokenIDClaim", parentTokenID.ToString());
            Claim requestIDClaim = new Claim("RequestID", requestID.ToString());

            return new [] { userIDClaim, parentTokenIDClaim, requestIDClaim };
        }

        public string GetTokenClaim(string token, string claimType)
        {
            JwtSecurityToken jwtSecurityToken = new JwtSecurityTokenHandler().ReadToken(token) as JwtSecurityToken;
            return jwtSecurityToken.Claims.ToList().Find(c => c.Type == claimType).Value;
        }
    }
}