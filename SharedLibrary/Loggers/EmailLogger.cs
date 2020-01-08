using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Newtonsoft.Json;

using DataAccessLayer.CommonInterfaces;
using DataAccessLayer.DatabasesManager;
using DataAccessLayer.Configuration.Interfaces;
using DataAccessLayer.Configuration;

using SharedLibrary.FactoriesManager;
using SharedLibrary.FactoriesManager.Interfaces;
using SharedLibrary.Loggers.Interfaces;
using SharedLibrary.Loggers.Models;
using SharedLibrary.Loggers.Configuration;
using System.Runtime.CompilerServices;

namespace SharedLibrary.Loggers
{
    public class EmailLogger : ILogger<EmailLog>
    {
        private readonly LogThread logThread;  

        public EmailLogger(string logConfigurationFilePath, int loggerID)
        {
            string logFilePath = Path.Combine(Path.GetDirectoryName(logConfigurationFilePath), $"email_log_{loggerID}.log");
            logThread = new LogThread(logFilePath, new LoggerConfiguration(logConfigurationFilePath));
        }

        #region ILogger implementation
        public void Log(EmailLog emailLog, LogLevel logLevel, LogConfiguration logConfiguration)
        {
            logThread.QueueLog(new Log(emailLog, logLevel, logConfiguration));
        }
        #endregion
    }
}
