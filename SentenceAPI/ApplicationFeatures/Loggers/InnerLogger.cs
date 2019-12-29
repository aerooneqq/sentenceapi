using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.Concurrent;

using SentenceAPI.ApplicationFeatures.Loggers.Configuration;
using SentenceAPI.ApplicationFeatures.Loggers.Models;


namespace SentenceAPI.ApplicationFeatures.Loggers
{
    public class InnerLogger
    {
        private volatile ConcurrentQueue<Log> logQueue;
        private IList<LogTask> logTasks;

        private string logConfigurationPath;
        public string LogConfigurationPath 
        {
            get => logConfigurationPath;
            private set 
            {
                if (!File.Exists(value))
                {
                    throw new FileNotFoundException("The file with logger configuration does not exists");
                }

                logConfigurationPath = value;
            }
        }

        private int fileCount;
        public int FileCount 
        { 
            get => fileCount;
            private set 
            {
                if (value < 0 || value > 10)
                {
                    throw new ArgumentException("The fileCount must be between 1 and 10");
                }

                fileCount = value;
            }
        }

        public LoggerConfiguration LoggerConfiguration { get; }

        public InnerLogger(string logConfigurationPath, string logFileName, int fileCount = 8)
        {
            FileCount = fileCount;
            LogConfigurationPath = logConfigurationPath;
            LoggerConfiguration = new LoggerConfiguration(LogConfigurationPath);
            logQueue = new ConcurrentQueue<Log>();
            logTasks = new List<LogTask>();

            string logDirectory = Path.GetDirectoryName(logConfigurationPath);
            for (int i = 0; i < fileCount; ++i)
            {
                string logFilePath = Path.Combine(logDirectory, $"{logFileName}_{i}.log");

                if (!File.Exists(logFilePath))
                {
                    using FileStream fs = File.Create(logFilePath);
                }   
            }

            for (int i = 0; i < fileCount; ++i)
            {
                string logFilePath = Path.Combine(logDirectory, $"{logFileName}_{i}.log");

                logTasks.Add(new LogTask(logFilePath, new LoggerConfiguration(LoggerConfiguration),
                    logQueue));
            }
        }

        public void QueueLog(Log log)
        {
            logQueue.Enqueue(log);
        }
    }
}