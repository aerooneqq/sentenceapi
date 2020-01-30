using System.IO;
using SharedLibrary.Loggers.Interfaces;

using Domain.Logs;
using Domain.Logs.Configuration;


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
