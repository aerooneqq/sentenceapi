using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SentenceAPI.Features.Users.Models;
using SentenceAPI.KernelInterfaces;

namespace SentenceAPI.Features.Email.Interfaces
{
    public interface IEmailService : IService
    {
        Task SendConfirmationEmail(string code, string email);
    }
}
