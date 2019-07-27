using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace SentenceAPI.Features.Authentication.Models
{
    /// <summary>
    /// Class where authentication parameters are stored
    /// </summary>
    public class AuthOptions
    {
        #region Public constants
        public const string ISSUER = "SentenceKernel";
        public const string AUDIENCE = "https://localhost:44368/";
        public const int SecondsLifeTime = 86400;
        public const int CustomClaimsCount = 3;
        #endregion

        #region Private constants
        const string SecretKey = "asdl;kf l;sdfksdl;f'k dsfl;kds;ldsl;kds;l sd';lfk sdl;'f d";
        #endregion

        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey));
        }
    }
}
