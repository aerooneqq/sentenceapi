using System;
using System.IO;
using Newtonsoft.Json;
using SentenceAPI.ApplicationFeatures.Loggers.Configuration;
using SentenceAPI.ApplicationFeatures.Loggers.Models;

namespace SentenceAPI.ApplicationFeatures.Loggers
{
    public static class DefaultLogger
    {
        private static object fileLocker = new object();
        private static FileStream fileStream;
        private static StreamWriter streamWriter;

        static DefaultLogger()
        {
            fileStream = new FileStream(Path.Combine(Directory.GetCurrentDirectory(), "log.txt"), FileMode.Create, FileAccess.Write);
            streamWriter = new StreamWriter(fileStream);
        }

        public static void Log(RequestLog requestLog, LogLevel logLevel)
        {
            Log log = new Log() 
            {
                Base64Data = null, 
                Date = DateTime.UtcNow, 
                JsonData = JsonConvert.SerializeObject(requestLog),
                LogLevel = logLevel, 
                Message = null, 
                Place = ComponentType.Service,
                XMLData = null
            };

            lock (fileLocker)
            {
                streamWriter.WriteLine(JsonConvert.SerializeObject(log));
            }
        }

        public static void Log(ResponseLog responseLog, LogLevel logLevel)
        {
            Log log = new Log() 
            {
                Base64Data = null, 
                Date = DateTime.UtcNow, 
                JsonData = JsonConvert.SerializeObject(responseLog),
                LogLevel = logLevel, 
                Message = null, 
                Place = ComponentType.Service,
                XMLData = null
            };

            lock (fileLocker)
            {
                streamWriter.WriteLine(JsonConvert.SerializeObject(log));
            }
        }

        public static void Log(ApplicationError errorLog, LogLevel logLevel)
        {
            Log log = new Log() 
            {
                Base64Data = null, 
                Date = DateTime.UtcNow, 
                JsonData = JsonConvert.SerializeObject(errorLog),
                LogLevel = logLevel, 
                Message = null, 
                Place = ComponentType.Service,
                XMLData = null
            };
            
            lock (fileLocker)
            {
                streamWriter.WriteLine(JsonConvert.SerializeObject(log));
            }
        }
    }
}