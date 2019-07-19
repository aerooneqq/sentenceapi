using System;

using SentenceAPI.Features.FactoriesManager.Models;

namespace SentenceAPI.Features.FactoriesManager.Interfaces
{
    public interface IFactoriesManager
    {
        void AddFactory(FactoryInfo factory);
        bool RemoveFactory(Type serviceType);

        FactoryInfo this[Type serviceType] { get; }
    }
}
