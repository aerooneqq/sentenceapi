using System;
using System.IO;

using Newtonsoft.Json;
using SharedLibrary.Loggers.Configuration;
using SharedLibrary.Loggers.Interfaces;
using SharedLibrary.Loggers.Models;

namespace SharedLibrary.Loggers
{
    public class ResponseLogger : ILogger<ResponseLog>
    {
        private LogThread logThread;

        /// <summary>
        /// This property must be initialized befote each logging
        /// </summary>
        public LogConfiguration LogConfiguration { get; set; }


        public ResponseLogger(string logConfigurationFilePath, int loggerID)
        {
            string logFilePath = Path.Combine(Path.GetDirectoryName(logConfigurationFilePath), $"response_log_{loggerID}.log");
            logThread = new LogThread(logFilePath, new LoggerConfiguration(logConfigurationFilePath));

            LogConfiguration = new LogConfiguration(typeof(object))
            {
                ComponentType = ComponentType.Undefined,
                ClassName = string.Empty,
            };
        }


        public void Log(ResponseLog responseLog, LogLevel logLevel, LogConfiguration logConfiguration)
        {
            logThread.QueueLog(new Log(responseLog, logLevel, logConfiguration));
        }
    }
}