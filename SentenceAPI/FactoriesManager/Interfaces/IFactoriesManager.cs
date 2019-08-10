using System;

using SentenceAPI.FactoriesManager.Models;

namespace SentenceAPI.FactoriesManager.Interfaces
{
    public interface IFactoriesManager
    {
        void AddFactory(FactoryInfo factory);
        bool RemoveFactory(Type factoryInfo);

        FactoryInfo this[Type factoryInfo] { get; }
    }
}
