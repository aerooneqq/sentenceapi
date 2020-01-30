using System;
using System.Collections.Generic;
using System.IO;

using Domain.Logs;

using SharedLibrary.Loggers;
using SharedLibrary.Loggers.Interfaces;


namespace DocumentsAPI.ApplicationFeatures.Loggers.Factories
{
    public class LoggerFactory : ILoggerFactory
    {
        private static Random Random { get; } = new Random();

        private static readonly IDictionary<Type, string> loggersPaths = new Dictionary<Type, string>()
        {
            [typeof(ExceptionLogger)] = Path.Combine(Startup.CurrDirectory, "log", "app_log", "log_conf.conf"),
            [typeof(RequestLogger)] = Path.Combine(Startup.CurrDirectory, "log", "request_log", "log_conf.conf"),
            [typeof(ResponseLogger)] = Path.Combine(Startup.CurrDirectory, "log", "response_log", "log_conf.conf")
        };

        private readonly IList<ExceptionLogger> exceptionLoggers;
        private readonly IList<RequestLogger> requestLoggers;
        private readonly IList<ResponseLogger> responseLoggers;

        public LoggerFactory()
        {
            exceptionLoggers = new List<ExceptionLogger>();
            responseLoggers = new List<ResponseLogger>();
            requestLoggers = new List<RequestLogger>();

            for (int i = 0; i < 5; ++i)
            {
                exceptionLoggers.Add(new ExceptionLogger(loggersPaths[typeof(ExceptionLogger)], i));
                responseLoggers.Add(new ResponseLogger(loggersPaths[typeof(ResponseLogger)], i));
                requestLoggers.Add(new RequestLogger(loggersPaths[typeof(RequestLogger)], i));
            }
        }

        public ILogger<ApplicationError> GetExceptionLogger()
        {
            return exceptionLoggers[Random.Next(exceptionLoggers.Count)];
        }

        public ILogger<EmailLog> GetEmailLogger()
        {
            throw new NotImplementedException();
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