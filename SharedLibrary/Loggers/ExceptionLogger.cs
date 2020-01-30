using System.IO;
using Domain.Logs;
using Domain.Logs.Configuration;

using SharedLibrary.Loggers.Interfaces;


namespace SharedLibrary.Loggers
{
    public class ExceptionLogger : ILogger<ApplicationError>
    {
        private readonly LogThread logThread;

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
