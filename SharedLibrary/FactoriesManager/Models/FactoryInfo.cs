using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Domain.KernelInterfaces;

using SharedLibrary.Extensions;


namespace SharedLibrary.FactoriesManager.Models
{
    public class FactoryInfo
    {
        /// <summary>
        /// Collection of services which are supported by the current factory
        /// </summary>
        private IDictionary<Type, (Type[] methodParamsTypes, MethodInvokerDelegate methodInvoker)> services;

        public IServiceFactory Factory { get; }
        public Type FactoryType { get; }

        public FactoryInfo(IServiceFactory factory, Type factoryType)
        {
            Factory = factory;
            FactoryType = factoryType;

            services = GetFactoriesServices();
        }

        private IDictionary<Type, (Type[] methodParams, MethodInvokerDelegate)> GetFactoriesServices()
        {
            MethodInfo[] methods = FactoryType.GetMethods();
            var services = new Dictionary<Type, (Type[] methodParams, MethodInvokerDelegate)>();

            foreach (MethodInfo method in methods)
            {
                var methodParams = method.GetParameters().Select(param => param.ParameterType).ToArray();
                services.Add(method.ReturnType, (methodParams, FactoryType.GetMethodDelegate(method.Name)));
            }

            return services;
        }

        public bool CheckIfFactorySupportService(Type serviceType) => services.Keys.ToArray().Count(
            type => type == serviceType || type.GetInterfaces().Contains(serviceType)) > 0;

        public ServiceType GetService<ServiceType>(Type serviceType, InjectionInfo[] injections)
        {
            Type[] methodParamsTypes = services[serviceType].methodParamsTypes;

            object[] finalMethodParams = new object[methodParamsTypes.Length];

            for (int i = 0; i < services[serviceType].methodParamsTypes.Length; ++i)
            {
                System.Console.WriteLine(serviceType + " " + methodParamsTypes[i]);
                Type paramType = methodParamsTypes[i];
                finalMethodParams[i] = injections.First(injection => injection.InterfaceType == paramType).Instance;
            }

            return (ServiceType)services[serviceType].methodInvoker(Factory, finalMethodParams);
        }
    }
}
