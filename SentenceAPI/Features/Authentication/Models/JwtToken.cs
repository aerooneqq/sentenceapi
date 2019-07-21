using SentenceAPI.KernelModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MongoDB.Bson.Serialization.Attributes;

using System.IdentityModel.Tokens.Jwt;

using Newtonsoft.Json;
using SentenceAPI.Features.Users.Models;
using System.Security.Claims;

namespace SentenceAPI.Features.Authentication.Models
{
    public class JwtToken : UniqueEntity
    {
        [BsonElement("userID"), JsonProperty("userID")]
        public long UserID { get; set; }

        [BsonElement("issuer"), JsonProperty("issuer")]
        public string Issuer { get; set; }

        [BsonElement("audience"), JsonProperty("audience")]
        public IEnumerable<string> Audiences { get; set; }

        [BsonElement("creationDate"), JsonProperty("creationDate")]
        public DateTime? ValidFrom { get; set; }

        [BsonElement("expiringDate"), JsonProperty("expiringDate")]
        public DateTime? ExpiringDate { get; set; }

        [BsonElement("signatureAlgorithm"), JsonProperty("signatureAlgorithm")]
        public string SignatureAlgorithm { get; set; }

        [BsonElement("claims"), JsonProperty("claims")]
        public List<KeyValuePair<string, string>> Claims { get; set; }

        public JwtToken(JwtSecurityToken token, UserInfo user)
        {
            UserID = user.ID;
            Issuer = token.Issuer;
            Audiences = token.Audiences;
            ValidFrom = token.ValidFrom;
            ExpiringDate = token.ValidTo;
            SignatureAlgorithm = token.SignatureAlgorithm;

            Claims = new List<KeyValuePair<string, string>>();
            var tokenClaims = token.Claims.ToList();

            for (int i = 0; i < AuthOptions.CustomClaimsCount; i++)
            {
                Claims.Add(new KeyValuePair<string, string>(tokenClaims[i].Type , tokenClaims[i].Value));
            }
        }
    }
}
