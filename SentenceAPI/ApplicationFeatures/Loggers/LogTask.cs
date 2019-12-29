using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;

using SentenceAPI.ApplicationFeatures.Loggers.Configuration;
using SentenceAPI.ApplicationFeatures.Loggers.Interfaces;
using SentenceAPI.ApplicationFeatures.Loggers.Models;
using System.Threading;

namespace SentenceAPI.ApplicationFeatures.Loggers
{
    public class LogTask
    {
        private volatile string logFilePath;
        private volatile LoggerConfiguration loggerConfiguration;
        private volatile ConcurrentQueue<Log> logQueue;

        public LogTask(string logFilePath, LoggerConfiguration loggerConfiguration, 
                       ConcurrentQueue<Log> logQueue)
        {
            this.logFilePath = logFilePath;
            this.loggerConfiguration = loggerConfiguration;
            this.logQueue = logQueue;

            Thread thread = new Thread(() => Log());
            thread.Start();
        }

        private void Log()
        {
            Log log;

            while (true)
            {
                System.Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId}");

                if (!logQueue.IsEmpty)
                {
                    bool dequeResult = logQueue.TryDequeue(out log);

                    if (dequeResult)
                    {
                        LogLogObject(log);
                    }
                }

                Thread.Sleep(100);
            } 
        }

        private void LogLogObject(Log log)
        {
            var supportedLogLevels = loggerConfiguration.LogLevelsConfiguration.Keys;

            if (supportedLogLevels.Contains(log.LogLevel))
            {
                using (FileStream fs = new FileStream(logFilePath, FileMode.Append, FileAccess.Write))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        sw.WriteLine(CreateLogString(log));
                    }
                }
            }
        }

        private string CreateLogString(Log log)
        {
            string logString = string.Empty;
            string delimiter = loggerConfiguration.LogLevelsConfiguration[log.LogLevel].Delimiter;
            IEnumerable<LogInformation> logData = loggerConfiguration.LogLevelsConfiguration[log.LogLevel].LogData;

            foreach (LogInformation logInformation in logData)
            {
                switch (logInformation)
                {
                    case LogInformation.LogLevel:
                        logString += delimiter.Insert(1, log.LogLevel.ToString());
                        break;

                    case LogInformation.Date:
                        logString += delimiter.Insert(1, log.Date.ToShortDateString());
                        break;
                    
                    case LogInformation.Time:
                        logString += delimiter.Insert(1, log.Date.ToLongTimeString());
                        break;

                    case LogInformation.DateTime:
                        logString += delimiter.Insert(1, log.Date.ToString("o"));
                        break;

                    case LogInformation.Message:
                        logString += delimiter.Insert(1, log.Message);
                        break;

                    case LogInformation.Place:
                        logString += delimiter.Insert(1, log.Place.ToString());
                        break;
                }   
            }

            logString += "\n";

            foreach (LogInformation logInformation in logData)
            {
                switch (logInformation)
                {
                    case LogInformation.ObjectJSONData:
                        logString += log.JsonData;
                        break;

                    case LogInformation.ObjectXMLData:
                        logString += log.XMLData;
                        break;

                    case LogInformation.ObjectBINARYData:
                        logString += log.Base64Data;
                        break;
                }
            }

            if (loggerConfiguration.LogLevelsConfiguration[log.LogLevel].ExtraLine)
            {
                logString += "\n";
            }

            return logString;
        }
    }
}