using System;
using System.Reflection;
using TinaX.UIKit.UIMessage;
using UnityEngine;

namespace TinaX.UIKit.Page.Controller
{
#nullable enable
    /// <summary>
    /// 默认的控制器反射提供者
    /// </summary>
    public class DefaultControllerReflectionProvider : IControllerReflectionProvider
    {
        private Type m_ObjectArrayType = typeof(object[]);
        private Type m_NullableType = typeof(Nullable<>);

        /// <summary>
        /// 给指定的对象发送消息
        /// </summary>
        /// <param name="controllerObject"></param>
        /// <param name="messageName"></param>
        /// <param name="messageArgs"></param>
        /// <returns>是否成功</returns>
        public bool TrySendMessage(object controllerObject, ref Type? controllerType, string messageName, object?[]? messageArgs)
        {
            controllerType ??= GetObjectType(ref controllerObject);
            var method = controllerType!.GetMethod(messageName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            if (method == null)
                return false; //反射没用
            if (method.GetCustomAttribute<NotUIMessageAttribute>() != null) //这个方法标注了“我不是UI消息方法啊”的信息
                return false;

            var methodParams = method.GetParameters();
            if(methodParams.Length == 0) //指定的方法没有参数
            {
                method.Invoke(controllerObject, null);
                return true;
            }

            if(methodParams.Length == 1)
            {
                var firstParam = methodParams[0];
                if(firstParam.ParameterType == m_ObjectArrayType)
                {
                    //只有唯一一个参数，是一个object[]，所以我们可以把所有数据全部传进去
                    var invokeArgs = new object[1] {messageArgs ?? new object[0]};
                    method.Invoke(controllerObject, invokeArgs);
                    return true;
                }
            }

            //定义的方法指定了多个参数，如果和传过来的args一一匹配，则调用
            if (messageArgs == null || messageArgs.Length == 0)
                return false;
            if (messageArgs.Length > methodParams.Length)
                return false;

            for(int i = 0; i < messageArgs.Length; i++)
            {
                var _arg = messageArgs[i];
                var _param = methodParams[i];
                var paramType = _param.ParameterType;
                if(_arg == null)
                {
                    //传入参数这个位置是null，我们看看定义的那边能不能接收null
                    if(paramType.IsValueType)
                    {
                        //是值类型，但有个特例是Nullable
                        if (!IsNullable(ref paramType))
                            return false; //不是的话就没办法了
                    }
                }
                else
                {
                    var argType = _arg.GetType();
                    //判断类型是否匹配
                    if(argType != paramType)
                    {
                        //两者不匹配，照理说无法注入，特例就是Nullable,Nullable会导致类型不一样
                        if (!TryGetNullableUnderlyingType(ref paramType, out var underlyingType))
                            return false; //不是Nullable，下面不用说了

                        if(argType != underlyingType)
                            return false;
                    }
                }
            }

            //上面一通判断下来都没问题的话，就调用它吧
            method.Invoke(controllerObject, messageArgs);
            return true;
        }




        protected virtual Type GetObjectType(ref object sourceObj)
        {
            return sourceObj.GetType();
        }


        protected bool IsNullable(ref Type type)
            => type.IsGenericType && type.GetGenericTypeDefinition() == m_NullableType;

        /// <summary>
        /// 尝试获取Nullable下的底层类型
        /// </summary>
        /// <returns>是否是Nullable类型</returns>
        protected bool TryGetNullableUnderlyingType(ref Type type, out Type underlyingType)
        {
            underlyingType = Nullable.GetUnderlyingType(type);
            return underlyingType != null;
        }

    }

#nullable restore
}
