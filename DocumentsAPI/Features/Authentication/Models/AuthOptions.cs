using System;
using System.Text;
using System.IO;
using System.Collections.Generic;

using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace DocumentsAPI.Features.Authentication.Models
{
    public static class AuthOptions
    {
        private readonly static string configPath = "./configs";
        
        static AuthOptions() 
        {
            using FileStream fileStream = new FileStream($"{configPath}/authentication_config.json", 
                FileMode.Open, FileAccess.Read);
            using StreamReader streamReader = new StreamReader(fileStream);

            string fileContent = streamReader.ReadToEnd();

            Dictionary<string, object> config = JsonConvert.DeserializeObject<Dictionary<string, object>>(fileContent);

            Issuer = (string)config[nameof(Issuer)];
            Audienece = (string)config[nameof(Audienece)];
            SecondsLifeTime = (int)config[nameof(SecondsLifeTime)];
            CustomClaimsCount = (int)config[nameof(CustomClaimsCount)];
            SecretKey = (string)config[nameof(SecretKey)];
        }


        public static string Issuer { get; }
        public static string Audienece { get; }
        public static int SecondsLifeTime { get; }
        public static int CustomClaimsCount { get; }
        private static string SecretKey { get; }


        public static SymmetricSecurityKey GetSecurityKey() => 
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