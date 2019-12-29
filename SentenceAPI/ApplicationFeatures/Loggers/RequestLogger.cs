using System;
using System.Diagnostics;
using System.IO;

using Newtonsoft.Json;

using SentenceAPI.ApplicationFeatures.Loggers.Configuration;
using SentenceAPI.ApplicationFeatures.Loggers.Interfaces;
using SentenceAPI.ApplicationFeatures.Loggers.Models;


namespace SentenceAPI.ApplicationFeatures.Loggers
{
    public class RequestLogger : ILogger<RequestLog>
    {
        private static string logConfigurationFilePath = Path.Combine(Startup.CurrDirectory, "log", 
            "request_log", "log_conf.conf");
        private volatile static InnerLogger innerLogger = new InnerLogger(logConfigurationFilePath, "request_log", 4);
            

        /// <summary>
        /// This property must be initialized befote each logging
        /// </summary>
        public LogConfiguration LogConfiguration { get; set; }


        public RequestLogger() 
        {
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

            innerLogger.QueueLog(log);
        }
    }
}