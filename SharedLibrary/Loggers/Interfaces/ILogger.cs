using System;
using System.Runtime.CompilerServices;

using SharedLibrary.Loggers.Configuration;
using SharedLibrary.Loggers.Models;
using SharedLibrary.KernelInterfaces;


namespace SharedLibrary.Loggers.Interfaces
{
    public interface ILogger<LogType> : IService
    {
        #region Methods
        void Log(LogType logObject, LogLevel logLevel, LogConfiguration logConfiguration);
        #endregion
    }
}
