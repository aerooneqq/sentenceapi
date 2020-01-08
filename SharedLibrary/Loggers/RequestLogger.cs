using System;
using System.IO;

using SharedLibrary.Loggers.Configuration;
using SharedLibrary.Loggers.Interfaces;
using SharedLibrary.Loggers.Models;

using Newtonsoft.Json;


namespace SharedLibrary.Loggers
{
    public class RequestLogger : ILogger<RequestLog>
    {
        private LogThread logThread;


        public RequestLogger(string logConfigurationFilePath, int loggerID) 
        {
            string logFilePath = Path.Combine(Path.GetDirectoryName(logConfigurationFilePath), $"request_log_{loggerID}.log");
            logThread = new LogThread(logFilePath, new LoggerConfiguration(logConfigurationFilePath));
        }


        public void Log(RequestLog requestLog, LogLevel logLevel, LogConfiguration logConfiguration)
        {
            logThread.QueueLog(new Log(requestLog, logLevel, logConfiguration));
        }
    }
}