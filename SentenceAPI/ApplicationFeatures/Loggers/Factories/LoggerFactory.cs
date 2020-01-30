using System;
using System.Collections.Generic;
using System.IO;

using SharedLibrary.Loggers.Interfaces;
using SharedLibrary.Loggers;

using Domain.Logs;


namespace SentenceAPI.ApplicationFeatures.Loggers.Factories
{
    public class LoggerFactory : ILoggerFactory
    {
        private static Random Random { get; } = new Random();

        private IDictionary<Type, string> LoggersPaths = new Dictionary<Type, string>()
        {
            [typeof(ExceptionLogger)] = Path.Combine(Startup.CurrDirectory, "log", "app_log", "log_conf.conf"),
            [typeof(RequestLogger)] = Path.Combine(Startup.CurrDirectory, "log", "request_log", "log_conf.conf"),
            [typeof(ResponseLogger)] = Path.Combine(Startup.CurrDirectory, "log", "response_log", "log_conf.conf"),
            [typeof(EmailLogger)] = Path.Combine(Startup.CurrDirectory, "log", "email_log", "log_conf.conf"),
        };

        private IList<ExceptionLogger> exceptionLoggers;
        private IList<RequestLogger> requestLoggers;
        private IList<ResponseLogger> responseLoggers;
        private IList<EmailLogger> emailLoggers;

        public LoggerFactory()
        {
            exceptionLoggers = new List<ExceptionLogger>();
            responseLoggers = new List<ResponseLogger>();
            requestLoggers = new List<RequestLogger>();
            emailLoggers = new List<EmailLogger>();

            for (int i = 0; i < 5; ++i)
            {
                exceptionLoggers.Add(new ExceptionLogger(LoggersPaths[typeof(ExceptionLogger)], i));
                responseLoggers.Add(new ResponseLogger(LoggersPaths[typeof(ResponseLogger)], i));
                requestLoggers.Add(new RequestLogger(LoggersPaths[typeof(RequestLogger)], i));
                emailLoggers.Add(new EmailLogger(LoggersPaths[typeof(EmailLogger)], i));
            }
        }

        public ILogger<ApplicationError> GetExceptionLogger()
        {
            return exceptionLoggers[Random.Next(exceptionLoggers.Count)];
        }

        public ILogger<EmailLog> GetEmailLogger()
        {
            return emailLoggers[Random.Next(emailLoggers.Count)];
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
