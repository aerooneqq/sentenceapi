using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SentenceAPI.Features.Loggers.Models;
using SentenceAPI.KernelInterfaces;

namespace SentenceAPI.Features.Loggers.Interfaces
{
    public interface ILogger : IService
    {
        #region Properties
        LogConfiguration LogConfiguration { get; set; }
        #endregion

        #region Methods
        Task Log(Exception ex);
        Task Log(string message);
        #endregion
    }
}
