﻿using System;
using System.Reflection;
using System.Linq;
using Domain.KernelInterfaces;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace Domain.Logs
{
    public class LogConfiguration
    {
        public string ClassName { get; set; }
        public ComponentType ComponentType { get; set; }
        public string MethodName { get; set; }


        public LogConfiguration(Type objectType, [CallerMemberName]string methodName = "")
        {
            ClassName = objectType.Name;
            ComponentType = GetComponentType(objectType);
            MethodName = methodName;
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

            if (objectType.BaseType == typeof(ControllerBase))
                return ComponentType.Controller;

            return ComponentType.Undefined;
        }
    }
}
