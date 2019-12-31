using System;
using System.Collections.Generic;

namespace SharedLibrary.Extensions
{
    public static class LinqExtensions
    {
        public static bool Contains<T>(this IEnumerable<T> collection, Predicate<T> predicate)
        {
            foreach (T t in collection)
            {
                if (predicate(t))
                    return true;
            }

            return false;
        }
    }
}