using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessLayer.Attributes
{
    /// <summary>
    /// Shows that the property is a secret one, and can not be included in any server response.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class SecretAttribute : Attribute
    {

    }
}
