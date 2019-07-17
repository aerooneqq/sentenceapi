using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace SentenceAPI.Features.Authentication.Models
{
    /// <summary>
    /// Class where authentication parameters are stored
    /// </summary>
    public class AuthOptions
    {
        public const string ISSUER = "SentenceKernel";
        public const string AUDIENCE = "";
        const string SecretKey = "asdl;kf l;sdfksdl;f'k dsfl;kds;ldsl;kds;l sd';lfk sdl;'f d";

        public const int SecondsLifeTime = 100;

        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey));
        }
    }
}
