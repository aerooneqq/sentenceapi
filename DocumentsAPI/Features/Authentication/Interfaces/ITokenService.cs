using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

using DocumentsAPI.Features.Authentication.Models;

using MongoDB.Bson;


namespace DocumentsAPI.Features.Authentication.Interfaces
{
    public interface ITokenService
    {
        Task InsertDocumentToken(DocumentsJwtToken token);
        (string encodedToken, JwtSecurityToken securityToken) CreateEncodedToken(ObjectId userID, 
                                                                                 ObjectId parentTokenID,
                                                                                 ObjectId requestID);
        string GetTokenClaim(string token, string claimType);
    }
}