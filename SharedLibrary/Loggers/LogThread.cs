using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Domain.Logs;
using Domain.Logs.Configuration;

namespace SharedLibrary.Loggers
{
    public class LogThread
    {
        private string logFilePath;
        private readonly LoggerConfiguration loggerConfiguration;
        private volatile ConcurrentQueue<Log> logQueue;
        private readonly Thread logThread; 
        private readonly FileStream logFileStream;
        private readonly StreamWriter logStreamWriter;
    

        public LogThread(string logFilePath, LoggerConfiguration loggerConfiguration)
        {
            this.logFilePath = logFilePath;
            this.loggerConfiguration = loggerConfiguration;
            logQueue = new ConcurrentQueue<Log>();
            
            logFileStream = new FileStream(logFilePath, FileMode.Append, FileAccess.Write);
            logStreamWriter = new StreamWriter(logFileStream);

            logThread = new Thread(() => Log());
            logThread.Start();
        }

        ~LogThread()
        {
            logStreamWriter.Dispose();
            logFileStream.Dispose();
        }


        public void QueueLog(Log log)
        {
            logQueue.Enqueue(log);
        }

        private void Log()
        {
            while (true)
            {
                if (logQueue.Count != 0)
                {
                    bool dequeResult = logQueue.TryDequeue(out Log log);

                    if (dequeResult)
                    {
                        LogLogObject(log);
                    }
                }
            } 
        }

        private void LogLogObject(Log log)
        {
            var supportedLogLevels = loggerConfiguration.LogLevelsConfiguration.Keys;

            if (supportedLogLevels.Contains(log.LogLevel))
            {
                logStreamWriter.WriteLine(CreateLogString(log));
                logStreamWriter.Flush();
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
                        logString += delimiter.Insert(1, log.Message ?? string.Empty);
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