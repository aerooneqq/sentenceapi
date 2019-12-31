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

        /// <summary>
        /// This property must be initialized befote each logging
        /// </summary>
        public LogConfiguration LogConfiguration { get; set; }


        public RequestLogger(string logConfigurationFilePath, int loggerID) 
        {
            string logFilePath = Path.Combine(Path.GetDirectoryName(logConfigurationFilePath), $"request_log_{loggerID}.log");
            logThread = new LogThread(logFilePath, new LoggerConfiguration(logConfigurationFilePath));

            LogConfiguration = new LogConfiguration(typeof(object))
            {
                ComponentType = ComponentType.Undefined,
                ClassName = string.Empty,
            };
        }


        public void Log(RequestLog logObject, LogLevel logLevel)
        {
            Log log = new Log() 
            {
                Base64Data = null, 
                Date = DateTime.UtcNow, 
                JsonData = JsonConvert.SerializeObject(logObject),
                LogLevel = logLevel, 
                Message = null, 
                Place = LogConfiguration.ComponentType,
                XMLData = null
            };

            logThread.QueueLog(log);
        }
    }
}