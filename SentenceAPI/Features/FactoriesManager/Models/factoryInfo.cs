using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SentenceAPI.KernelInterfaces;

namespace SentenceAPI.Features.FactoriesManager.Models
{
    public class FactoryInfo
    {
        public IFactory Factory { get; }
        public Type ServiceType { get; }

        public FactoryInfo(IFactory factory, Type serviceType)
        {
            Factory = factory;
            ServiceType = serviceType;
        }
    }
}
