using System;
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
    }
}