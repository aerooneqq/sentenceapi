using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SentenceAPI.KernelInterfaces;

namespace SentenceAPI.Features.Loggers.Interfaces
{
    public interface ILoggerFactory : IFactory
    {
        ILogger GetLogger();
    }
}
