using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using System.Reflection.Emit;

namespace SharedLibrary.Extensions
{
    public delegate void PropertyGetValueDelegate(object obj);
    
    ///<summary>
    ///
    ///</summary>
    public static class TypeExtensions
    {
        public static MethodInvokerDelegate GetMethodDelegate(this Type type, string methodName)
        {
            MethodInfo method = type.GetMethod(methodName);

            if (method is null)
            {
                throw new ArgumentException($"The method {methodName} does not exist in {type.FullName}");
            }

            IList<Type> paramsTypes = method.GetParameters().Select(p => p.ParameterType).ToList();

            if (paramsTypes.Count > 3)
            {
                throw new NotSupportedException();
            }

            Type o = typeof(object);
            DynamicMethod dynamicMethod = new DynamicMethod(methodName, o, 
                                                            paramsTypes.Prepend(o).Select(_ => o).ToArray());

            ILGenerator iLGenerator = dynamicMethod.GetILGenerator();

            iLGenerator.Emit(OpCodes.Ldarg, 0);

            for (int i = 0; i < paramsTypes.Count; ++i)
            {
                iLGenerator.Emit(OpCodes.Ldarg, i + 1);

                if (paramsTypes[i].IsValueType)
                {
                    iLGenerator.Emit(OpCodes.Unbox_Any, paramsTypes[i]);
                }
            }

            iLGenerator.Emit(OpCodes.Call, method);

            if (method.ReturnType.IsValueType)
            {
                iLGenerator.Emit(OpCodes.Box, method.ReturnType);
            }

            iLGenerator.Emit(OpCodes.Ret);

            return MethodDelegateBuilder.GetDelegate(dynamicMethod);
        }
    }
}