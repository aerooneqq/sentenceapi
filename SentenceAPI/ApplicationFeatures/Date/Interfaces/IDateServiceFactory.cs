using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SharedLibrary.KernelInterfaces;

namespace SentenceAPI.ApplicationFeatures.Date.Interfaces
{
    public interface IDateServiceFactory : IServiceFactory
    {
        IDateService GetService();
    }
}
