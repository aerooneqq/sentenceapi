﻿using System.Threading.Tasks;
using System.IO;
using System;

using Newtonsoft.Json;
using System.Threading;
using SharedLibrary.Loggers.Interfaces;
using SharedLibrary.Loggers.Models;
using SharedLibrary.Loggers.Configuration;

namespace SharedLibrary.Loggers
{
    public class ExceptionLogger : ILogger<ApplicationError>
    {
        private LogThread logThread;

        #region Properties
        /// <summary>
        /// This property must be initialized before logging
        /// </summary>
        public LogConfiguration LogConfiguration { get; set; }
        #endregion

        #region Constructors
        public ExceptionLogger(string logConfigurationFilePath, int loggerID) 
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
        public void Log(ApplicationError applicationError, LogLevel logLevel, LogConfiguration logConfiguration)
        {
            logThread.QueueLog(new Log(applicationError, logLevel, logConfiguration));
        }
        #endregion
    }
}
