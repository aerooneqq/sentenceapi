using System.Threading.Tasks;
using System.IO;

using SentenceAPI.ApplicationFeatures.Loggers.Models;
using SentenceAPI.ApplicationFeatures.Loggers.Interfaces;
using SentenceAPI.ApplicationFeatures.Loggers.Configuration;
using System;

using Newtonsoft.Json;
using System.Threading;

namespace SentenceAPI.ApplicationFeatures.Loggers
{
    public class ExceptionLogger : ILogger<ApplicationError>
    {
        private static string logConfigurationFilePath = Path.Combine(Startup.CurrDirectory, "log", 
            "app_log", "log_conf.conf");
        //private volatile static InnerLogger innerLogger = new InnerLogger(logConfigurationFilePath, "app_log", 4);
        private LogThread logThread;

        #region Properties
        /// <summary>
        /// This property must be initialized before logging
        /// </summary>
        public LogConfiguration LogConfiguration { get; set; }
        #endregion

        #region Constructors
        public ExceptionLogger(int loggerID) 
        {
            string logFilePath = Path.Combine(Path.GetDirectoryName(logConfigurationFilePath), $"app_log_{loggerID}.log");
            logThread = new LogThread(logFilePath, new LoggerConfiguration(logConfigurationFilePath));

            LogConfiguration = new LogConfiguration(typeof(object))
            {
                ClassName = string.Empty, 
                ComponentType = ComponentType.Undefined
            };
        }
        #endregion

        #region ILogger implmentation
        public void Log(ApplicationError logObject, LogLevel logLevel)
        {
            Log log = new Log() 
            {
                Base64Data = null, 
                Date = DateTime.UtcNow, 
                JsonData = JsonConvert.SerializeObject(logObject),
                LogLevel = logLevel, 
                Message = logObject.Message, 
                Place = LogConfiguration.ComponentType,
                XMLData = null
            };

            logThread.QueueLog(log);
        }
        #endregion
    }
}
