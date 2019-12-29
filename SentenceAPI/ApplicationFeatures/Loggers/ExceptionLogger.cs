﻿using System.Threading.Tasks;
using System.IO;

using SentenceAPI.ApplicationFeatures.Loggers.Models;
using SentenceAPI.ApplicationFeatures.Loggers.Interfaces;
using SentenceAPI.ApplicationFeatures.Loggers.Configuration;
using System;
using Newtonsoft.Json;

namespace SentenceAPI.ApplicationFeatures.Loggers
{
    public class ExceptionLogger : ILogger<ApplicationError>
    {
        private static string logConfigurationFilePath = Path.Combine(Startup.CurrDirectory, "log", 
            "app_log", "log_conf.conf");
        private static InnerLogger innerLogger = new InnerLogger(logConfigurationFilePath, "app_log", 3);

        #region Properties
        /// <summary>
        /// This property must be initialized before logging
        /// </summary>
        public LogConfiguration LogConfiguration { get; set; }
        #endregion

        #region Constructors
        public ExceptionLogger() 
        {
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

            innerLogger.QueueLog(log);
        }
        #endregion
    }
}
