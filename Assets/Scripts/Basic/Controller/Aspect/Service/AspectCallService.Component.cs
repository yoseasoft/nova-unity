/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyring (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
///
/// Permission is hereby granted, free of charge, to any person obtaining a copy
/// of this software and associated documentation files (the "Software"), to deal
/// in the Software without restriction, including without limitation the rights
/// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
/// copies of the Software, and to permit persons to whom the Software is
/// furnished to do so, subject to the following conditions:
///
/// The above copyright notice and this permission notice shall be included in
/// all copies or substantial portions of the Software.
///
/// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
/// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
/// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
/// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
/// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
/// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
/// THE SOFTWARE.
/// -------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Reflection;

using SystemType = System.Type;
using SystemAction = System.Action;
using SystemDelegate = System.Delegate;
using SystemMethodInfo = System.Reflection.MethodInfo;
using SystemParameterInfo = System.Reflection.ParameterInfo;

namespace GameEngine
{
    /// <summary>
    /// 提供切面访问接口的服务类，对整个程序内部的对象实例提供切面访问的服务逻辑处理
    /// </summary>
    public static partial class AspectCallService
    {
        [OnServiceProcessRegisterOfTarget(typeof(CComponent), AspectBehaviourType.Initialize)]
        private static void CallServiceProcessOfComponentInitialize(CComponent component, bool reload)
        {
            // Debugger.Info("Register component '{0}' event and message listener with target behaviour type '{1}'.", component.GetType().FullName, AspectBehaviourType.Initialize.ToString());

            RegComponentEventAndMessageListenerByTargetType(component, AspectBehaviourType.Initialize, reload);
        }

        [OnServiceProcessRegisterOfTarget(typeof(CComponent), AspectBehaviourType.Startup)]
        private static void CallServiceProcessOfComponentStartup(CComponent component, bool reload)
        {
            // Debugger.Info("Register component '{0}' event and message listener with target behaviour type '{1}'.", component.GetType().FullName, AspectBehaviourType.Startup.ToString());

            RegComponentEventAndMessageListenerByTargetType(component, AspectBehaviourType.Startup, reload);
        }

        [OnServiceProcessRegisterOfTarget(typeof(CComponent), AspectBehaviourType.Awake)]
        private static void CallServiceProcessOfComponentAwake(CComponent component, bool reload)
        {
            // Debugger.Info("Register component '{0}' event and message listener with target behaviour type '{1}'.", component.GetType().FullName, AspectBehaviourType.Awake.ToString());

            RegComponentEventAndMessageListenerByTargetType(component, AspectBehaviourType.Awake, reload);
        }

        [OnServiceProcessRegisterOfTarget(typeof(CComponent), AspectBehaviourType.Start)]
        private static void CallServiceProcessOfComponentStart(CComponent component, bool reload)
        {
            // Debugger.Info("Register component '{0}' event and message listener with target behaviour type '{1}'.", component.GetType().FullName, AspectBehaviourType.Start.ToString());

            RegComponentEventAndMessageListenerByTargetType(component, AspectBehaviourType.Start, reload);
        }

        private static void RegComponentEventAndMessageListenerByTargetType(CComponent component, AspectBehaviourType behaviourType, bool reload)
        {
            SystemType targetType = component.GetType();
            Loader.GeneralCodeInfo codeInfo = Loader.CodeLoader.LookupGeneralCodeInfo(targetType, typeof(CComponent));
            if (null == codeInfo)
            {
                Debugger.Warn("Could not found any aspect call component service process with target type '{0}', called it failed.", targetType.FullName);
                return;
            }

            Loader.ComponentCodeInfo componentCodeInfo = codeInfo as Loader.ComponentCodeInfo;
            if (null == componentCodeInfo)
            {
                Debugger.Warn("The aspect call component service process getting error code info '{0}' with target type '{1}', called it failed.", codeInfo.GetType().FullName, targetType.FullName);
                return;
            }

            // 订阅事件信息
            for (int n = 0; n < componentCodeInfo.GetEventSubscribingMethodTypeCount(); ++n)
            {
                Loader.EventSubscribingMethodTypeCodeInfo methodTypeCodeInfo = componentCodeInfo.GetEventSubscribingMethodType(n);
                if (methodTypeCodeInfo.BehaviourType != behaviourType) continue;

                if (false == NovaEngine.Utility.Reflection.IsTypeOfExtension(methodTypeCodeInfo.Method) && reload)
                {
                    // 针对对象内部的成员函数，在重载模式下不能对其撤销后再次注册
                    continue;
                }

                // SystemDelegate callback = NovaEngine.Utility.Reflection.CreateGenericActionDelegate(targetObject, methodTypeCodeInfo.Method);
                // Debugger.Assert(null != callback, "Invalid method type.");

                Debugger.Info(LogGroupTag.Controller, "Register component '{0}' event listener with target method '{1}'.", targetType.FullName, NovaEngine.Utility.Text.ToString(methodTypeCodeInfo.Method));

                if (methodTypeCodeInfo.EventID > 0)
                {
                    // if (reload) { component.Unsubscribe(methodTypeCodeInfo.EventID); }

                    component.Subscribe(methodTypeCodeInfo.EventID, methodTypeCodeInfo.Method, true);
                }
                else
                {
                    // if (reload) { component.Unsubscribe(methodTypeCodeInfo.EventDataType); }

                    component.Subscribe(methodTypeCodeInfo.EventDataType, methodTypeCodeInfo.Method, true);
                }
            }

            // 消息绑定信息
            for (int n = 0; n < componentCodeInfo.GetMessageBindingMethodTypeCount(); ++n)
            {
                Loader.MessageBindingMethodTypeCodeInfo methodTypeCodeInfo = componentCodeInfo.GetMessageBindingMethodType(n);
                if (methodTypeCodeInfo.BehaviourType != behaviourType) continue;

                if (false == NovaEngine.Utility.Reflection.IsTypeOfExtension(methodTypeCodeInfo.Method) && reload)
                {
                    // 针对对象内部的成员函数，在重载模式下不能对其撤销后再次注册
                    continue;
                }

                // SystemDelegate callback = NovaEngine.Utility.Reflection.CreateGenericActionDelegate(targetObject, methodTypeCodeInfo.Method);
                // Debugger.Assert(null != callback, "Invalid method type.");

                Debugger.Info(LogGroupTag.Controller, "Register component '{0}' message listener with target method '{1}'.", targetType.FullName, NovaEngine.Utility.Text.ToString(methodTypeCodeInfo.Method));

                if (methodTypeCodeInfo.Opcode > 0)
                {
                    // if (reload) { component.RemoveMessageListener(methodTypeCodeInfo.Opcode); }

                    component.AddMessageListener(methodTypeCodeInfo.Opcode, methodTypeCodeInfo.Method, true);
                }
                else
                {
                    // if (reload) { component.RemoveMessageListener(methodTypeCodeInfo.MessageType); }

                    component.AddMessageListener(methodTypeCodeInfo.MessageType, methodTypeCodeInfo.Method, true);
                }
            }
        }
    }
}
