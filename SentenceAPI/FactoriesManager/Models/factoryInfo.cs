using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SentenceAPI.KernelInterfaces;

namespace SentenceAPI.FactoriesManager.Models
{
    public class FactoryInfo
    {
        public IFactory Factory { get; }
        public Type FactoryType { get; }

        public FactoryInfo(IFactory factory, Type factoryType)
        {
            Factory = factory;
            FactoryType = factoryType;
        }
    }
}
