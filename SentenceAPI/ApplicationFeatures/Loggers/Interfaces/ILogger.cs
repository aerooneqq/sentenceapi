using SentenceAPI.ApplicationFeatures.Loggers.Configuration;
using SentenceAPI.ApplicationFeatures.Loggers.Models;

using SharedLibrary.KernelInterfaces;


namespace SentenceAPI.ApplicationFeatures.Loggers.Interfaces
{
    public interface ILogger<LogType> : IService
    {
        #region Properties
        LogConfiguration LogConfiguration { get; set; }
        #endregion

        #region Methods
        void Log(LogType logObject, LogLevel logLevel);
        #endregion
    }
}
