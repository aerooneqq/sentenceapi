﻿using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

namespace Domain.Authentication
{
    #warning FIX THIS HARDCODED SHIT
    
    /// <summary>
    /// Class where authentication parameters are stored
    /// </summary>
    public static class AuthOptions
    {
        #region Public constants
        public const string ISSUER = "SentenceAuthorization";
        public const string AUDIENCE = "https://localhost:44368/";
        public const int SecondsLifeTime = 86400;
        public const int CustomClaimsCount = 5;
        #endregion

        #region Private constants
        const string SecretKey = "asdl;kf l;sdfksdl;f'k dsfl;kds;ldsl;kds;l sd';lfk sdl;'f d";
        #endregion

        public static SymmetricSecurityKey GetSymmetricSecurityKey() => 
            new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey));

        public static LifetimeValidator GetLifeTimeValidationDel()
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

    }
}
