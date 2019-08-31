using System;

using SentenceAPI.FactoriesManager.Models;
using SentenceAPI.KernelInterfaces;

namespace SentenceAPI.FactoriesManager.Interfaces
{
    public interface IFactoriesManager
    {
        void AddFactory(FactoryInfo factory);
        bool RemoveFactory(Type factoryInfo);

        IServiceFactory this[Type factoryInfo] { get; }
    }
}
