using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SentenceAPI.Extensions
{
    public static class TypeExtensions
    {
        /// <summary>
        /// Gets the value of the BsonElementAttribute for the given instance of type T.
        /// </summary>
        /// <typeparam name="T">The type of the model</typeparam>
        /// <exception cref="ArgumentException">
        /// When the property does not exist or the property does not have the BsonElementAttribute.
        /// </exception>"
        public static string GetBsonPropertyName(this Type type, string propertyName)
        {
            var property = type.GetProperty(propertyName);

            if (property == null)
            {
                throw new ArgumentException($"The property name {propertyName} is not valid for the model" +
                    $" {type.FullName}");
            }

            var bsonElementNameAttr = property.GetCustomAttribute<BsonElementAttribute>();

            if (bsonElementNameAttr == null)
            {
                throw new ArgumentException($"The property {propertyName} does not have the BsonElementAttribute");
            }

            return bsonElementNameAttr.ElementName;
        }

        /// <summary>
        /// Gets the property depending on the BSON property name
        /// </summary>
        /// <returns>
        /// The PropertyInfo object if the property under the given name exists,
        /// NULL otherwise
        /// </returns>
        public static PropertyInfo GetPropertyFromBSONName(this Type type, string bsonPropertyName)
        {
            PropertyInfo[] properties = type.GetTypeInfo().GetProperties(BindingFlags.Public 
                | BindingFlags.Instance);

            return properties.Where(p =>
            {
                BsonElementAttribute bsonElementAttribute = p.GetCustomAttribute<BsonElementAttribute>();

                if (bsonElementAttribute != null)
                {
                    return bsonElementAttribute.ElementName == bsonPropertyName;
                }

                return false;
            }).FirstOrDefault();
        }

        /// <summary>
        /// Gets the property depending on the JSON property name.
        /// </summary>
        /// <returns>
        /// The PropertyInfo object of the property under the given JSON name exists,
        /// NULL otherwise
        /// </returns>
        public static PropertyInfo GetPropertyFromJsonName(this Type type, string jsonPropertyName)
        {
            PropertyInfo[] properties = type.GetTypeInfo().GetProperties(BindingFlags.Public |
                BindingFlags.Instance);

            return properties.Where(p =>
            {
                JsonPropertyAttribute jsonPropertyAttribute = p.GetCustomAttribute<JsonPropertyAttribute>();

                if (jsonPropertyAttribute != null )
                {
                    return jsonPropertyAttribute.PropertyName == jsonPropertyName;
                }

                return false;
            }).FirstOrDefault();
        }
    }
}
