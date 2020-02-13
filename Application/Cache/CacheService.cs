using System;
using System.Collections.Generic;
using Application.Caching.Interfaces;


namespace Application.Caching
{
    public class CacheService : ICacheService
    {
        private readonly IDictionary<string, object> storage;


        public CacheService()
        {
            storage = new Dictionary<string, object>();
        }

        /// <summary>
        /// Tries to insert the given pair in the cache
        /// </summary>
        /// <exception cref="ArgumentNullException">When key or obj is null</exception>
        /// <returns>
        /// True if the insertion was successful, false otherwise
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