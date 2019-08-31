using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SentenceAPI.Features.Email.Interfaces;
using SentenceAPI.Features.Email.Services;
using SentenceAPI.KernelInterfaces;

namespace SentenceAPI.Features.Email.Factories
{
    public class EmailServiceFactory : IEmailServiceFactory
    {
        public IEmailService GetService()
        {
            return new EmailService();
        }
    }
}
