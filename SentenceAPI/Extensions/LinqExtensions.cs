using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SentenceAPI.Extensions
{
    public static class LinqExtensions
    {
        public static IEnumerable<Dictionary<string, object>> ConfigureNewObjects<T>(this IEnumerable<T> collection, 
            IEnumerable<string> propertiesNames)
        {
            return collection.Select(e => e.ConfigureNewObject(propertiesNames));
        }
    }
}
