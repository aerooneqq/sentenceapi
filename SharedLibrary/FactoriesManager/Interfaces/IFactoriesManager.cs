using System;

using SharedLibrary.FactoriesManager.Models;


namespace SharedLibrary.FactoriesManager.Interfaces
{
    public interface IFactoriesManager
    {
        void AddFactory(FactoryInfo factory);
        bool RemoveFactory(Type factoryInfo);

        void Inject(Type interfaceType, object instance);

        WeakReference<IType> GetService<IType>() 
            where IType : class;
    }
}
