using SentenceAPI.ApplicationFeatures.Loggers.Interfaces;
using SentenceAPI.ApplicationFeatures.Loggers;
using SentenceAPI.ApplicationFeatures.Loggers.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentenceAPI.ApplicationFeatures.Loggers.Factories
{
    public class LoggerFactory : ILoggerFactory
    {
        private static Random Random { get; } = new Random();

        private IList<ExceptionLogger> exceptionLoggers;
        private IList<RequestLogger> requestLoggers;
        private IList<ResponseLogger> responseLoggers;

        public LoggerFactory()
        {
            exceptionLoggers = new List<ExceptionLogger>();
            responseLoggers = new List<ResponseLogger>();
            requestLoggers = new List<RequestLogger>();

            for (int i = 0; i < 5; ++i)
            {
                exceptionLoggers.Add(new ExceptionLogger(i));
                responseLoggers.Add(new ResponseLogger(i));
                requestLoggers.Add(new RequestLogger(i));
            }
        }

        public ILogger<ApplicationError> GetExceptionLogger()
        {
            return exceptionLoggers[Random.Next(exceptionLoggers.Count)];
        }

        public ILogger<EmailLog> GetEmailLogger()
        {
            return new EmailLogger();
        }

        public ILogger<RequestLog> GetRequestLog()
        {
            return requestLoggers[Random.Next(requestLoggers.Count)];
        }

        public ILogger<ResponseLog> GetResponseLog()
        {
            return responseLoggers[Random.Next(responseLoggers.Count)];
        }
    }
}
