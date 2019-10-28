using System;

using SharedLibrary.FactoriesManager.Models;
using SharedLibrary.KernelInterfaces;

namespace SharedLibrary.FactoriesManager.Interfaces
{
    public interface IFactoriesManager
    {
        void AddFactory(FactoryInfo factory);
        bool RemoveFactory(Type factoryInfo);

        WeakReference<IType> GetService<IType>() where IType : class;
    }
}
