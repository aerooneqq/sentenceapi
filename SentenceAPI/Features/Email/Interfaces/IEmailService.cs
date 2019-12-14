using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SentenceAPI.Features.Users.Models;
using SharedLibrary.KernelInterfaces;

namespace SentenceAPI.Features.Email.Interfaces
{
    public interface IEmailService : IService
    {
        Task SendConfirmationEmailAsync(string code, string email);
    }
}
