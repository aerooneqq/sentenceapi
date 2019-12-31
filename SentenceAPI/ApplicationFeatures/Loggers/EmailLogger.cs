using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Newtonsoft.Json;

using SentenceAPI.ApplicationFeatures.Loggers.Interfaces;
using SentenceAPI.ApplicationFeatures.Loggers.Models;
using SentenceAPI;

using DataAccessLayer.CommonInterfaces;
using DataAccessLayer.DatabasesManager;
using DataAccessLayer.Configuration.Interfaces;
using DataAccessLayer.Configuration;

using SharedLibrary.FactoriesManager;
using SharedLibrary.FactoriesManager.Interfaces;
using SentenceAPI.ApplicationFeatures.Loggers.Configuration;

namespace SentenceAPI.ApplicationFeatures.Loggers
{
    public class EmailLogger : ILogger<EmailLog>
    {
        #region Static fields
        private static string logConfigurationFilePath = Path.Combine(Startup.CurrDirectory, "log", 
            "email_log", "log_conf.conf");

        private readonly LogThread logThread;  
        #endregion

        #region Properties

        /// <summary>
        /// This property must be initialized befote each logging
        /// </summary>
        public LogConfiguration LogConfiguration { get; set; }
        #endregion

        public EmailLogger(int loggerID)
        {
            string logFilePath = Path.Combine(Path.GetDirectoryName(logConfigurationFilePath), $"email_log_{loggerID}.log");
            logThread = new LogThread(logFilePath, new LoggerConfiguration(logConfigurationFilePath));

            LogConfiguration = new LogConfiguration(typeof(object))
            {
                ClassName = string.Empty, 
                ComponentType = ComponentType.Undefined
            };
        }

        #region ILogger implementation
        public void Log(EmailLog logObject, LogLevel logLevel)
        {
            Log log = new Log()
            {
                LogLevel = logLevel,
                Date = DateTime.UtcNow,
                JsonData = JsonConvert.SerializeObject(logObject),
                Place = LogConfiguration.ComponentType,
                Message = string.Empty,
                Base64Data = null,
                XMLData = null,
            }; 

            logThread.QueueLog(log);
        }
        #endregion
    }
}
