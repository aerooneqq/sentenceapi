using SentenceAPI.KernelInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentenceAPI.Features.Email.Interfaces
{
    public interface IEmailServiceFactory : IServiceFactory
    {
        IEmailService GetService();
    }
}
