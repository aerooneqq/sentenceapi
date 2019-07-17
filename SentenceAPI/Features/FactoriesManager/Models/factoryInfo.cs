using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentenceAPI.Features.FactoryManager.Models
{
    public class FactoryInfo
    {
        public Type InterfaceFactoryType { get; }
        public Type ImplementaionFactoryType { get; }
    }
}
