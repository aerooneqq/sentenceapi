using SentenceAPI.Features.Users.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentenceAPI.Features.Links.Interfaces
{
    public interface ILinkService
    {
        Task<string> CreateVerificationLinkAsync(UserInfo user);
        Task<bool?> ActivateLinkAsync(string link);
    }
}
