using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using SentenceAPI.KernelInterfaces;

namespace SentenceAPI.FactoriesManager.Models
{
    public class FactoryInfo
    {
        /// <summary>
        /// Collection of services which are supported by the current factory
        /// </summary>
        private IDictionary<Type, MethodInfo> services;

        public IServiceFactory Factory { get; }
        public Type FactoryType { get; }

        public FactoryInfo(IServiceFactory factory, Type factoryType)
        {
            Factory = factory;
            FactoryType = factoryType;

            services = GetFactoriesServices();
        }

        private IDictionary<Type, MethodInfo> GetFactoriesServices()
        {
            MethodInfo[] methods = FactoryType.GetMethods();
            Dictionary<Type, MethodInfo> services = new Dictionary<Type, MethodInfo>();

            foreach (MethodInfo method in methods)
            {
                services.Add(method.ReturnType, method);
            }

            return services;
        }

        public bool CheckIfFactorySupportService(Type serviceType) => services.Keys.Contains(serviceType);

        public ServiceType GetService<ServiceType>(Type serviceType)
        {
            return (ServiceType)services[serviceType].Invoke(Factory, new object[] { });
        }
    }
}
