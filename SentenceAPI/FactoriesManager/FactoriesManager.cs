using System;
using System.Collections.Generic;
using System.Linq;

using SentenceAPI.FactoriesManager.Interfaces;
using SentenceAPI.FactoriesManager.Models;
using SentenceAPI.KernelInterfaces;

namespace SentenceAPI.FactoriesManager
{
    public class FactoriesManager : IFactoriesManager
    {
        #region Fields
        private List<FactoryInfo> factoryInfos;
        #endregion

        #region Constructors
        private FactoriesManager()
        {
            factoryInfos = new List<FactoryInfo>();
        }
        #endregion

        #region IFactoryManager implementaion
        /// <summary>
        /// Returns the factory with a given interface type. If the factory not in
        /// the list returns null.
        /// </summary>
        /// <param name="t">
        /// Type of the interface.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the factoryType parameter was NULL
        /// </exception>
        /// <returns>
        /// NULL if the value of the weak reference doesn't contain a value
        /// </returns>
        public WeakReference<ServiceType> GetService<ServiceType>() where ServiceType : class
        {
            foreach (FactoryInfo factory in factoryInfos)
            {
                if (factory.CheckIfFactorySupportService(typeof(ServiceType)))
                {
                    return new WeakReference<ServiceType>(factory.GetService<ServiceType>(typeof(ServiceType)));
                }
            }

            return null; 
        }

        /// <summary>
        /// Tries to add the given factory to the factory list.
        /// If the factory is null then the argument exception is thrown.
        /// </summary>
        /// <param name="factory">The FactoryInfo object.</param>
        /// <exception cref="ArgumentNullException">When the factory is null</exception>
        public void AddFactory(FactoryInfo factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException("Factory object can not be null.");
            }

            if (CheckIfFactoryAlreadyAdded(factory))
            {
                throw new ArgumentException("The factory info with a given parameters" +
                    " already exists in a manager");
            }

            factoryInfos.Add(factory);
        }

        private bool CheckIfFactoryAlreadyAdded(FactoryInfo factory)
        {
            return factoryInfos.FindIndex(f =>
            {
                if (f.FactoryType == factory.FactoryType)
                {
                    return true;
                }

                return false;
            }) != -1;
        }

        /// <summary>
        /// Removes the factory from the factory list.
        /// </summary>
        /// <exception cref="ArgumentNullException">When the factory is null</exception>
        /// <returns>True if the removal was successful, false otherwise</returns>
        public bool RemoveFactory(Type factoryType)
        {
            if (factoryType == null)
                throw new ArgumentNullException("Service type can not be null.");

            return factoryInfos.Remove(factoryInfos.Find(f => f.FactoryType == factoryType));
        }
        #endregion

        #region Singleton
        private static FactoriesManager factoriesManager;
        public static FactoriesManager Instance
        {
            get
            {
                if (factoriesManager == null)
                    factoriesManager = new FactoriesManager();

                return factoriesManager;
            }
        }
        #endregion
    }
}
