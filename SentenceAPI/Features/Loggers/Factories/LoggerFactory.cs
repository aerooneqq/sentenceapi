using SentenceAPI.Features.Loggers.Interfaces;
using SentenceAPI.Features.Logger;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentenceAPI.Features.Loggers.Factories
{
    public class LoggerFactory : ILoggerFactory
    {
        public ILogger GetLogger()
        {
            return new ExceptionLogger();
        }
    }
}
