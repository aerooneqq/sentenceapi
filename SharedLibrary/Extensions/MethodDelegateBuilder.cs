using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Linq;

namespace SharedLibrary.Extensions
{
    public delegate object MethodInvokerDelegate(object contextObj, params object[] args);

    public class MethodDelegateBuilder
    {
        public static MethodInvokerDelegate GetDelegate(DynamicMethod dynamicMethod)
        {
            IList<Type> methodParams = dynamicMethod.GetParameters().Select(param => param.ParameterType)
                .ToList();

            switch (methodParams.Count)
            {
                case 0:
                    var func0 = (Func<object>)dynamicMethod.CreateDelegate(typeof(Func<object>)); 
                    return (contextObj, args) => func0();

                case 1:
                    var func1 = (Func<object, object>)dynamicMethod.CreateDelegate(typeof(Func<object, object>));
                    return (contextObj, args) => func1(contextObj);

                case 2:
                    Type funcDelegateType = typeof(Func<object, object, object>);
                    var func2 = (Func<object, object, object>)dynamicMethod.CreateDelegate(funcDelegateType);
                    return (contextObj, args) => func2(contextObj, args[0]);
                
                case 3:
                    funcDelegateType = typeof(Func<object, object, object, object>);
                    var func3 = (Func<object, object, object, object>)dynamicMethod.CreateDelegate(funcDelegateType);
                    return (contextObj, args) => func3(contextObj, args[0], args[1]);
                
                default:
                    throw new NotSupportedException($"Don't support methods with {methodParams.Count} parameters");
            }
        }
    }
}