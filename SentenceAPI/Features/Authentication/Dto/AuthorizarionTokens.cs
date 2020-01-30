using Newtonsoft.Json;

namespace SentenceAPI.Features.Authentication.Dto
{
    public class AuthorizarionTokens
    {
        [JsonProperty("sentenceAPIToken")]
        public string SentenceAPIToken { get; set; }
        
        [JsonProperty("documentsAPIToken")]
        public string DocumentsAPIToken { get; set; }


        public AuthorizarionTokens() { }
        public AuthorizarionTokens(string sentenceAPIToken, string documentsAPIToken)
        {
            SentenceAPIToken = sentenceAPIToken;
            DocumentsAPIToken = documentsAPIToken;
        }
    }
}