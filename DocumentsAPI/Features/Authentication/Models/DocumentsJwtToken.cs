using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;

using DataAccessLayer.KernelModels;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

using Newtonsoft.Json;

using DataAccessLayer.JsonConverters;


namespace DocumentsAPI.Features.Authentication.Models
{
    public class DocumentsJwtToken : UniqueEntity
    {
        [BsonElement("userID"), JsonProperty("userID")]
        [JsonConverter(typeof(ObjectIDJsonConverter))]
        public ObjectId UserID { get; set; }

        [BsonElement("parentSentenceAPITokenID"), JsonProperty("parentSentenceAPITokenID")]
        [JsonConverter(typeof(ObjectIDJsonConverter))]
        public ObjectId SentenceAPITokenID { get; set; }

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


        public DocumentsJwtToken() { }
        public DocumentsJwtToken(ObjectId userID, ObjectId sentenceAPITokenID, 
                                 JwtSecurityToken token)
        {
            ID = ObjectId.GenerateNewId(); 
            UserID = userID;
            SentenceAPITokenID = sentenceAPITokenID;
            Issuer = token.Issuer;
            Audiences = token.Audiences;
            ValidFrom = token.ValidFrom;
            ExpiringDate = token.ValidTo;
            SignatureAlgorithm = token.SignatureAlgorithm;
        }
    }
}