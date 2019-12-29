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
        private static InnerLogger innerLogger = new InnerLogger(logConfigurationFilePath, "email_log", 5);
        #endregion

        #region Properties

        /// <summary>
        /// This property must be initialized befote each logging
        /// </summary>
        public LogConfiguration LogConfiguration { get; set; }
        #endregion

        public EmailLogger()
        {
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

            innerLogger.QueueLog(log);
        }
        #endregion
    }
}
