using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SentenceAPI.Features.Loggers.Models;
using SentenceAPI.KernelInterfaces;

namespace SentenceAPI.Features.Loggers.Interfaces
{
    public interface ILogger<LogType> : IService
    {
        #region Properties
        string FileName { get; }
        LogConfiguration LogConfiguration { get; set; }
        #endregion

        #region Methods
        Task Log(LogType logObject);
        Task WriteLogToFile(LogType logType);
        #endregion
    }
}
