using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SharedLibrary.Extensions;

namespace SharedLibrary.FactoriesManager.Models
{
    public class InjectionInfo
    {
        public Type InterfaceType { get; }
        public object Instance { get; }


        /// <summary>
        /// 
        /// </summary>
        public InjectionInfo(Type interfaceType, object instance)
        {
            if (interfaceType is null || instance is null)
            {
                throw new ArgumentNullException("One of the arguments was null.");
            }

            if (!instance.GetType().GetInterfaces().Contains(interfaceType))
            {
                throw new ArgumentException($"The {instance.GetType().FullName} does not implement {interfaceType}");
            }

            InterfaceType = interfaceType;
            Instance = instance;
        }
    }
}