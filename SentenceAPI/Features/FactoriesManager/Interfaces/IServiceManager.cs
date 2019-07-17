using SentenceAPI.Features.FactoryManager.Models;
using System;

namespace SentenceAPI.Features.FactoryManager.Interfaces
{
    public interface IFactoryManager
    {
        void AddFactory(FactoryInfo factory);
        void RemoveFactory(FactoryInfo factory);

        FactoryInfo this[Type t] { get; }
    }
}
