using System.Collections.Generic;

namespace SentenceAPI.ApplicationFeatures.Loggers.Configuration
{
    public class LogLevelConfiguration
    {
        public string Delimiter { get; set; }
        public bool ExtraLine { get; set; }
        public IList<LogInformation> LogData { get; set; }

        
        public LogLevelConfiguration() 
        {
            LogData = new List<LogInformation>();
            Delimiter = " ";
        }
        
        public LogLevelConfiguration(string delimiter, bool extraLine, IList<LogInformation> logData)
        {
            Delimiter = delimiter; 
            ExtraLine = extraLine;
            LogData = logData;
        }

        public LogLevelConfiguration(LogLevelConfiguration logLevelConfiguration)
        {
            Delimiter = logLevelConfiguration.Delimiter;
            ExtraLine = logLevelConfiguration.ExtraLine;
            LogData = new List<LogInformation>(logLevelConfiguration.LogData);
        }
    }
}