using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SharedLibrary.KernelInterfaces;
using Microsoft.AspNetCore.Mvc;

namespace SharedLibrary.Loggers.Models
{
    public class LogConfiguration
    {
        public string ClassName { get; set; }

        public ComponentType ComponentType { get; set; }

        public LogConfiguration(Type objectType)
        {
            ClassName = objectType.Name;
            ComponentType = GetComponentType(objectType);
        }

        private ComponentType GetComponentType(Type objectType) 
        {
            Type[] implementedInterfaces = objectType.GetTypeInfo().GetInterfaces();

            if (implementedInterfaces.Contains(typeof(IService)))
                return ComponentType.Service;

            if (implementedInterfaces.Contains(typeof(IServiceFactory)))
                return ComponentType.Factory;

            if (implementedInterfaces.Contains(typeof(IValidator)))
                return ComponentType.Validator;

            if (objectType.BaseType == typeof(Controller) || objectType.BaseType == typeof(ControllerBase))
                return ComponentType.Controller;

            return ComponentType.Undefined;
        }
    }
}
