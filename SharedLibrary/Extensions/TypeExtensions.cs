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
        private static OpCode[] LdArgsCodes { get; } = 
            { OpCodes.Ldarg_0, OpCodes.Ldarg_1, OpCodes.Ldarg_2, OpCodes.Ldarg_3 }; 

        public static T InvokeMethod<T>(this Type type, string getterMethodName)
        {
            MethodInfo getterMethod = type.GetMethod(getterMethodName);
            DynamicMethod dynamicMethod = new DynamicMethod("GetValue", typeof(object),
                new Type[] { typeof(object) }, typeof(object), true);
            ILGenerator iLGenerator = dynamicMethod.GetILGenerator();

            //load the first argument (object obj)
            iLGenerator.Emit(OpCodes.Ldarg_0);
            //call getter
            iLGenerator.Emit(OpCodes.Call, getterMethod);

            //box the value type
            if (getterMethod.ReturnType.GetTypeInfo().IsValueType)
            {
                iLGenerator.Emit(OpCodes.Box, getterMethod.ReturnType);
            }

            //return result
            iLGenerator.Emit(OpCodes.Ret);

            PropertyGetValueDelegate getPropValueDel = 
                dynamicMethod.CreateDelegate(typeof(PropertyGetValueDelegate)) as PropertyGetValueDelegate;

            return default(T);
        }

        public static TDelegate GetMethodDelegate<TDelegate>(this Type type, string methodName)
            where TDelegate : Delegate
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

            iLGenerator.Emit(OpCodes.Ldarg_0);
            iLGenerator.Emit(OpCodes.Castclass, type);

            for (int i = 0; i < paramsTypes.Count; ++i)
            {
                iLGenerator.Emit(LdArgsCodes[i + 1]);

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

            return dynamicMethod.CreateDelegate(typeof(TDelegate)) as TDelegate;
        }
    }
}