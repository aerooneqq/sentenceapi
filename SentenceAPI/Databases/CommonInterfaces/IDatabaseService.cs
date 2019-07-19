using SentenceAPI.KernelInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentenceAPI.Databases.CommonInterfaces
{
    public interface IDatabaseService : IService
    {
        Task Connect();
        Task Disconnect();
    }
}
