using System;
using SentenceAPI.ApplicationFeatures.Loggers.Configuration;

namespace SentenceAPI.ApplicationFeatures.Loggers.Models
{
    public class Log
    {
        public LogLevel LogLevel { get; set; }
        public DateTime Date { get; set; }
        public string Message { get; set; }
        public ComponentType Place { get; set; }
        public string PlaceName { get; set; }
        public string JsonData { get; set; }
        public string XMLData { get; set; }
        public string Base64Data { get; set; }
    }
}