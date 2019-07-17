using SentenceAPI.KernelModels;

namespace SentenceAPI.Features.Users.Models
{
    public class UserInfo : UniqueEntity
    {
        public string Login { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
