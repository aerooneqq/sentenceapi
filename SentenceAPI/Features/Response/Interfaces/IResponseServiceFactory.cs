using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SentenceAPI.KernelInterfaces;

namespace SentenceAPI.Features.Response.Interfaces
{
    public interface IResponseServiceFactory : IFactory
    {
        IResponseService GetService();
    }
}
