using Newtonsoft.Json;

namespace SentenceAPI.Features.Authentication.Models
{
    public class AuthenticationResultDto
    {
        [JsonProperty("sentenceApiToken")]
        public string SentenceAPIToken { get; set; }

        [JsonProperty("documentsApiToken")]
        public string DocumentsAPIToken { get; set; }
    }
}