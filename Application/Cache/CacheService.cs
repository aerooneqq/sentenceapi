﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLibrary.Caching
{
    public class CacheService : ICacheService
    {
        #region Singleton
        static CacheService() { }

        private static ICacheService cacheService = null;
        public static ICacheService Service
        {
            get
            {
                if (cacheService is null)
                {
                    cacheService = new CacheService();
                }   

                return cacheService;
            }
        }

        private CacheService()
        {
            storage = new Dictionary<string, object>();
        }
        #endregion


        private IDictionary<string, object> storage;

        /// <summary>
        /// Tries to insert the given pair in the cache
        /// </summary>
        /// <exception cref="ArgumentNullException">When key or obj is null</exception>
        /// <returns>
        /// True if the insertion was successfull, false otherwise
        /// </returns>
        public bool TryInsert(string key, object obj)
        {
            if (key is null || obj is null)
            {
                throw new ArgumentNullException();
            }

            if (storage.ContainsKey(key))
            {
                return false;
            }

            storage.Add(key, obj);
            return true;
        }

        /// <summary>
        /// Checks if the key has a value
        /// </summary>
        /// <exception cref="ArgumentNullException">If the key is null</exception>
        public bool Contains(string key)
        {
            return storage.ContainsKey(key);
        }

        public object GetValue(string key)
        {
            return storage[key];
        }
    }
}
