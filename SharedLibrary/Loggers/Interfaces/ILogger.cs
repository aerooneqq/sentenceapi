using Domain.KernelInterfaces;
using Domain.Logs;
using Domain.Logs.Configuration;


namespace SharedLibrary.Loggers.Interfaces
{
    public interface ILogger<in LogType> : IService
    {
        #region Methods
        void Log(LogType logObject, LogLevel logLevel, LogConfiguration logConfiguration);
        #endregion
    }
}
