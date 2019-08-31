using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SentenceAPI.KernelInterfaces;

namespace SentenceAPI.FactoriesManager.Models
{
    public class FactoryInfo
    {
        public WeakReference<IServiceFactory> Factory { get; }
        public Type FactoryType { get; }

        public FactoryInfo(IServiceFactory factory, Type factoryType)
        {
            Factory = new WeakReference<IServiceFactory>(factory);
            FactoryType = factoryType;
        }
    }
}
