using MongoDB.Bson.Serialization.Attributes;

using SentenceAPI.Attributes;

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SentenceAPI.Extensions
{
    public static class GenericExtensions
    {
        /// <summary>
        /// Creates a dictionary from an object where only the properties which a listed in the propertiesNames are present.
        /// If the property marked with a SECRET attribute, then it will not be included the dictionary.
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, object> ConfigureNewObject<T>(this T obj, IEnumerable<string> propertiesNames)
        {
            IEnumerable<PropertyInfo> properties = typeof(T).GetTypeInfo().GetProperties()
                .Where(p => propertiesNames.Select(propertyName => propertyName.ToLowerInvariant())
                    .Contains(p.Name.ToLowerInvariant()));

            Dictionary<string, object> newObject = new Dictionary<string, object>();

            foreach (PropertyInfo property in properties)
            {
                if (property.GetCustomAttribute<SecretAttribute>() == null)
                {
                    newObject.Add(property.Name[0].ToString().ToLowerInvariant() + property.Name.Substring(1), 
                        property.GetValue(obj));
                }
            }

            return newObject;
        }
    }
}
