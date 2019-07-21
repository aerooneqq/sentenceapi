using System;

using SentenceAPI.Features.FactoriesManager.Models;

namespace SentenceAPI.Features.FactoriesManager.Interfaces
{
    public interface IFactoriesManager
    {
        void AddFactory(FactoryInfo factory);
        bool RemoveFactory(Type factoryInfo);

        FactoryInfo this[Type factoryInfo] { get; }
    }
}
