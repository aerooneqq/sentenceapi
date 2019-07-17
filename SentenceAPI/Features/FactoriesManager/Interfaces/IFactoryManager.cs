using System;

using SentenceAPI.Features.FactoriesManager.Models;

namespace SentenceAPI.Features.FactoriesManager.Interfaces
{
    public interface IFactoryManager
    {
        void AddFactory(FactoryInfo factory);
        bool RemoveFactory(FactoryInfo factory);

        FactoryInfo this[Type t] { get; }
    }
}
