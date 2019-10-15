using SentenceAPI.KernelInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentenceAPI.ApplicationFeatures.Date.Interfaces
{
    public interface IDateServiceFactory : IServiceFactory
    {
        IDateService GetService();
    }
}
