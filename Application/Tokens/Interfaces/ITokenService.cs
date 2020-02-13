using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

using Domain.Authentication;
using Domain.Users;

using Domain.KernelInterfaces;


namespace Application.Tokens.Interfaces
{   
    public interface ITokenService : IService
    {
        bool CheckToken(string encodedToken);
        (string encodedToken, JwtSecurityToken securityToken) CreateEncodedToken(UserInfo user);
        Task InsertTokenInDBAsync(JwtToken token);
        string GetTokenClaim(string token, string claimType);
        JwtSecurityToken DecodeToken(string token);
    }
}
