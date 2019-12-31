using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using SharedLibrary.KernelInterfaces;
using SharedLibrary.Extensions;

namespace SharedLibrary.FactoriesManager.Models
{
    public class FactoryInfo
    {
        /// <summary>
        /// Collection of services which are supported by the current factory
        /// </summary>
        private IDictionary<Type, Func<object, object>> services;

        public IServiceFactory Factory { get; }
        public Type FactoryType { get; }

        public FactoryInfo(IServiceFactory factory, Type factoryType)
        {
            Factory = factory;
            FactoryType = factoryType;

            services = GetFactoriesServices();
        }

        private IDictionary<Type, Func<object, object>> GetFactoriesServices()
        {
            MethodInfo[] methods = FactoryType.GetMethods();
            Dictionary<Type, Func<object, object>> services = new Dictionary<Type, Func<object, object>>();

            foreach (MethodInfo method in methods)
            {
                services.Add(method.ReturnType, FactoryType.GetMethodDelegate<Func<object, object>>(method.Name));
            }

            return services;
        }

        public bool CheckIfFactorySupportService(Type serviceType) => services.Keys.ToArray().Count(
            type => type == serviceType || type.GetInterfaces().Contains(serviceType)) > 0;

        public ServiceType GetService<ServiceType>(Type serviceType)
        {
            return (ServiceType)services[serviceType](Factory);
        }
    }
}
