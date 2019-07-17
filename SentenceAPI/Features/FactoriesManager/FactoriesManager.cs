using System;
using System.Collections.Generic;
using System.Linq;

using SentenceAPI.Features.FactoriesManager.Interfaces;
using SentenceAPI.Features.FactoriesManager.Models;

namespace SentenceAPI.Features.FactoriesManager
{
    public class FactoriesManager : IFactoryManager
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
        public FactoryInfo this[Type factoryInterfaceType]
        {
            get
            {
                return factoryInfos.FirstOrDefault(f => f.ServiceType == factoryInterfaceType);
            }
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
                throw new ArgumentNullException("Factory object can not be null.");

            factoryInfos.Add(factory);
        }

        /// <summary>
        /// Removes the factory from the factory list.
        /// </summary>
        /// <param name="factory"></param>
        /// <exception cref="ArgumentNullException">When the factory is null</exception>
        /// <returns>True if the removal was successful, false otherwise</returns>
        public bool RemoveFactory(FactoryInfo factory)
        {
            if (factory == null)
                throw new ArgumentNullException("Factory can not be null.");

            return factoryInfos.Remove(factory);
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
