using System;
using System.Collections.Generic;
using System.Linq;

using SentenceAPI.FactoriesManager.Interfaces;
using SentenceAPI.FactoriesManager.Models;

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
        public FactoryInfo this[Type factoryType]
        {
            get
            {
                if (factoryType == null)
                {
                    throw new ArgumentNullException("Service type can not be null.");
                }

                return factoryInfos.FirstOrDefault(f => f.FactoryType == factoryType);
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

            if (factoryInfos.FindIndex(f =>
            {
                if (f.FactoryType == factory.FactoryType || f.Factory.GetType() == factory.Factory.GetType())
                {
                    return true; 
                }

                return false;
            }) != -1)
            {
                throw new ArgumentException("The factory info with a given parameters" +
                    " already exists in a manager");
            }

            factoryInfos.Add(factory);
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
