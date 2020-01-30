using System;

using Newtonsoft.Json;

using Domain.Logs.Configuration;


namespace Domain.Logs
{
    public class Log
    {
        #region Properties
        [JsonProperty("logLevel")]
        public LogLevel LogLevel { get; set; }

        [JsonProperty("date")]
        public DateTime Date { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("place")]
        public ComponentType Place { get; set; }

        [JsonProperty("className")]
        public string ClassName { get; set; }

        [JsonProperty("methodName")]
        public string MethodName { get; set; }

        [JsonProperty("placeName")]
        public string PlaceName { get; set; }

        [JsonProperty("jsonData")]
        public string JsonData { get; set; }

        [JsonProperty("xmlData")]
        public string XMLData { get; set; }

        [JsonProperty("base64Data")]
        public string Base64Data { get; set; }
        #endregion

        #region Constructors
        private Log(LogLevel logLevel)
        {
            LogLevel = logLevel;
        }

        private Log(LogConfiguration logConfiguration, LogLevel logLevel) : this(logLevel)
        {
            Place = logConfiguration.ComponentType;
            ClassName = logConfiguration.ClassName;
        }

        public Log(RequestLog requestLog, LogLevel logLevel, LogConfiguration logConfiguration)
            : this(logConfiguration, logLevel)
        {
            InitializeLog<RequestLog>(requestLog);
        }

        private void InitializeLog<T>(T logObject)
        {
            Base64Data = null;
            Date = DateTime.UtcNow; 
            JsonData = JsonConvert.SerializeObject(logObject);
            Message = null;
            XMLData = null;
        }

        public Log(EmailLog emailLog, LogLevel logLevel, LogConfiguration logConfiguration)
            : this(logConfiguration, logLevel)
        {
            InitializeLog<EmailLog>(emailLog);
        }

        public Log(ResponseLog responseLog, LogLevel logLevel, LogConfiguration logConfiguration)
            : this(logConfiguration, logLevel)
        {
            InitializeLog<ResponseLog>(responseLog);
        }

        public Log(ApplicationError applicationError, LogLevel logLevel, LogConfiguration logConfiguration)
            : this(logConfiguration, logLevel)
        {
            InitializeLog<ApplicationError>(applicationError);
        }
        #endregion
    }
}