using SharedLibrary.Loggers.Configuration;
using SharedLibrary.Loggers.Models;

using SharedLibrary.KernelInterfaces;


namespace SharedLibrary.Loggers.Interfaces
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
