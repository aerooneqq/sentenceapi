using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Domain.Logs.Configuration
{
    public class LoggerConfiguration
    {
        private const string LogLevelPrefix = "LL";
        private const string DelimiterPrefix = "Del";
        private const string ExtraLinePrefix = "ExtraLine";
        private const string LogInformationPrefix = "LogInformation";

        private static string[] prefixes = { LogLevelPrefix, DelimiterPrefix, ExtraLinePrefix, LogInformationPrefix };

        private static readonly Dictionary<string, LogLevel> stringToLogLevelDict = 
            new Dictionary<string, LogLevel>
            {
                ["Trace"] = LogLevel.Trace,
                ["Debug"] = LogLevel.Debug,
                ["Information"] = LogLevel.Information,
                ["Warning"] = LogLevel.Warning,
                ["Error"] = LogLevel.Error,
                ["Critical"] = LogLevel.Critical,
            };
        private static readonly Dictionary<string, LogInformation> stringToLogInformationDict = 
            new Dictionary<string, LogInformation> 
            {
                ["Date"] = LogInformation.Date,
                ["Time"] = LogInformation.Time,
                ["DateTime"] = LogInformation.DateTime,
                ["LogLevel"] = LogInformation.LogLevel,
                ["Message"] = LogInformation.Message,
                ["Place"] = LogInformation.Place,
                ["ObjectJSONData"] = LogInformation.ObjectJSONData,
                ["ObjectXMLData"] = LogInformation.ObjectXMLData,
                ["ObjectBINARYData"] = LogInformation.ObjectBINARYData,
            };
        

        public IDictionary<LogLevel, LogLevelConfiguration> LogLevelsConfiguration { get; private set; }


        public LoggerConfiguration(LoggerConfiguration loggerConfiguration)
        {
            LogLevelsConfiguration = new Dictionary<LogLevel, LogLevelConfiguration>(); 

            foreach (var (logLevel, logConfiguration) in loggerConfiguration.LogLevelsConfiguration)
            {
                LogLevelsConfiguration.Add(logLevel, new LogLevelConfiguration(logConfiguration));
            }
        }

        public LoggerConfiguration(string filePath) 
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"The file with name {filePath} does not exist");
            }

            LogLevelsConfiguration = new Dictionary<LogLevel, LogLevelConfiguration>();

            ReadLogConfigurationFile(File.ReadAllLines(filePath));
        }

        private void ReadLogConfigurationFile(string[] lines) 
        {
            for (int i = 0; i < lines.Length;) 
            {
                LogLevel currLogLevel = LogLevel.Undefined;

                if (lines[i].Length == 0)
                {
                    ++i;
                    continue;
                }

                while (i < lines.Length && lines[i].Length != 0) 
                {
                    string line = DeleteAllSpaeces(lines[i]);
                    string linePrefix = line.Substring(0, line.IndexOf('='));

                    switch (linePrefix)
                    {
                        case LogLevelPrefix:
                            string logLevelType = line.Substring(line.IndexOf(LogLevelPrefix, StringComparison.Ordinal)
                                                                 + LogLevelPrefix.Length + 1);
                            currLogLevel = GetLogLevelFromString(logLevelType);
                            
                            if (LogLevelsConfiguration.ContainsKey(currLogLevel))
                            {
                                ThrowInvalidDataException();
                            }

                            LogLevelsConfiguration.Add(currLogLevel, new LogLevelConfiguration());

                            break;

                        case DelimiterPrefix:
                            ThrowIfLogLevelUndefined(currLogLevel);
                            
                            string delimiter = line.Substring(line.IndexOf(DelimiterPrefix, StringComparison.Ordinal)
                                                              + DelimiterPrefix.Length + 1);

                            if (delimiter is {} && !string.IsNullOrEmpty(delimiter))
                            {
                                LogLevelsConfiguration[currLogLevel].Delimiter = delimiter;
                            }

                            break;

                        case ExtraLinePrefix:
                            ThrowIfLogLevelUndefined(currLogLevel);

                            string extraLine = line.Substring(line.IndexOf(ExtraLinePrefix) + ExtraLinePrefix.Length + 1);
                            
                            if (extraLine is {} && !string.IsNullOrEmpty(extraLine))
                            {
                                try
                                {
                                    LogLevelsConfiguration[currLogLevel].ExtraLine = bool.Parse(extraLine);
                                }
                                catch (FormatException)
                                {
                                    ThrowInvalidDataException();
                                }
                            }

                            break;

                        case LogInformationPrefix:
                            ThrowIfLogLevelUndefined(currLogLevel);

                            string logDataString = line.Substring(line.IndexOf(LogInformationPrefix, StringComparison.Ordinal) 
                                                                  + LogInformationPrefix.Length + 1);
                            
                            string[] logData = logDataString.Split(',');

                            foreach (string logInformation in logData)
                            {
                                LogLevelsConfiguration[currLogLevel].LogData.Add(GetLogInformation(logInformation));
                            }

                            break;
                    }
                    ++i;
                }
            }
        }

        private string DeleteAllSpaeces(string line) 
        {
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < line.Length; ++i)
            {
                if (line[i] != ' ')
                {
                    stringBuilder.Append(line[i]);
                }
            }

            return stringBuilder.ToString();
        }

        private LogLevel GetLogLevelFromString(string logLevel)
        {
            try 
            {
                return stringToLogLevelDict[logLevel];
            }
            catch
            {
                ThrowInvalidDataException();
                return LogLevel.Undefined;
            }
        }

        private void ThrowIfLogLevelUndefined(LogLevel logLevel)
        {
            if (logLevel == LogLevel.Undefined)
            {
                ThrowInvalidDataException();
            }
        }

        private void ThrowInvalidDataException()
        {
            throw new InvalidDataException("The log_conf file is not of the right scheme");
        }

        private LogInformation GetLogInformation(string logInformationString)
        {
            try 
            {
                return stringToLogInformationDict[logInformationString];
            }
            catch
            {
                ThrowInvalidDataException();
                return LogInformation.Undefined;
            }
        }
    }
}