using MongoDB.Bson.Serialization.Attributes;
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
    }
}
