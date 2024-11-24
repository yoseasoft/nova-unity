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
        [OnServiceProcessRegisterOfTarget(typeof(CRef), AspectBehaviourType.Initialize)]
        private static void CallServiceProcessOfRefInitialize(CRef obj, bool reload)
        {
            // Debugger.Info("Register ref '{0}' event and message listener with target behaviour type '{1}'.", obj.GetType().FullName, AspectBehaviourType.Initialize.ToString());

            RegRefEventAndMessageListenerByTargetType(obj, AspectBehaviourType.Initialize, reload);
        }

        [OnServiceProcessRegisterOfTarget(typeof(CRef), AspectBehaviourType.Startup)]
        private static void CallServiceProcessOfRefStartup(CRef obj, bool reload)
        {
            // Debugger.Info("Register ref '{0}' event and message listener with target behaviour type '{1}'.", obj.GetType().FullName, AspectBehaviourType.Startup.ToString());

            RegRefEventAndMessageListenerByTargetType(obj, AspectBehaviourType.Startup, reload);
        }

        [OnServiceProcessRegisterOfTarget(typeof(CRef), AspectBehaviourType.Awake)]
        private static void CallServiceProcessOfRefAwake(CRef obj, bool reload)
        {
            // Debugger.Info("Register ref '{0}' event and message listener with target behaviour type '{1}'.", obj.GetType().FullName, AspectBehaviourType.Awake.ToString());

            RegRefEventAndMessageListenerByTargetType(obj, AspectBehaviourType.Awake, reload);
        }

        [OnServiceProcessRegisterOfTarget(typeof(CRef), AspectBehaviourType.Start)]
        private static void CallServiceProcessOfRefStart(CRef obj, bool reload)
        {
            // Debugger.Info("Register ref '{0}' event and message listener with target behaviour type '{1}'.", obj.GetType().FullName, AspectBehaviourType.Start.ToString());

            RegRefEventAndMessageListenerByTargetType(obj, AspectBehaviourType.Start, reload);
        }

        private static void RegRefEventAndMessageListenerByTargetType(CRef obj, AspectBehaviourType behaviourType, bool reload)
        {
            SystemType targetType = obj.GetType();
            Loader.GeneralCodeInfo codeInfo = Loader.CodeLoader.LookupGeneralCodeInfo(targetType, typeof(CRef));
            if (null == codeInfo)
            {
                Debugger.Warn("Could not found any aspect call ref service process with target type '{0}', called it failed.", targetType.FullName);
                return;
            }

            Loader.RefCodeInfo refCodeInfo = codeInfo as Loader.RefCodeInfo;
            if (null == refCodeInfo)
            {
                Debugger.Warn("The aspect call ref service process getting error code info '{0}' with target type '{1}', called it failed.", codeInfo.GetType().FullName, targetType.FullName);
                return;
            }

            // 订阅事件信息
            for (int n = 0; n < refCodeInfo.GetEventSubscribingMethodTypeCount(); ++n)
            {
                Loader.EventSubscribingMethodTypeCodeInfo methodTypeCodeInfo = refCodeInfo.GetEventSubscribingMethodType(n);
                if (methodTypeCodeInfo.BehaviourType != behaviourType) continue;

                if (false == NovaEngine.Utility.Reflection.IsTypeOfExtension(methodTypeCodeInfo.Method) && reload)
                {
                    // 针对对象内部的成员函数，在重载模式下不能对其撤销后再次注册
                    continue;
                }

                // SystemDelegate callback = NovaEngine.Utility.Reflection.CreateGenericActionDelegate(targetObject, methodTypeCodeInfo.Method);
                // Debugger.Assert(null != callback, "Invalid method type.");

                Debugger.Info(LogGroupTag.Controller, "Register ref '{0}' event listener with target method '{1}'.", targetType.FullName, NovaEngine.Utility.Text.ToString(methodTypeCodeInfo.Method));

                if (methodTypeCodeInfo.EventID > 0)
                {
                    // if (reload) { obj.Unsubscribe(methodTypeCodeInfo.EventID, methodTypeCodeInfo.Method); }

                    obj.Subscribe(methodTypeCodeInfo.EventID, methodTypeCodeInfo.Method, true);
                }
                else
                {
                    // if (reload) { obj.Unsubscribe(methodTypeCodeInfo.EventDataType, methodTypeCodeInfo.Method); }

                    obj.Subscribe(methodTypeCodeInfo.EventDataType, methodTypeCodeInfo.Method, true);
                }
            }

            // 消息绑定信息
            for (int n = 0; n < refCodeInfo.GetMessageBindingMethodTypeCount(); ++n)
            {
                Loader.MessageBindingMethodTypeCodeInfo methodTypeCodeInfo = refCodeInfo.GetMessageBindingMethodType(n);
                if (methodTypeCodeInfo.BehaviourType != behaviourType) continue;

                if (false == NovaEngine.Utility.Reflection.IsTypeOfExtension(methodTypeCodeInfo.Method) && reload)
                {
                    // 针对对象内部的成员函数，在重载模式下不能对其撤销后再次注册
                    continue;
                }

                // SystemDelegate callback = NovaEngine.Utility.Reflection.CreateGenericActionDelegate(targetObject, methodTypeCodeInfo.Method);
                // Debugger.Assert(null != callback, "Invalid method type.");

                Debugger.Info(LogGroupTag.Controller, "Register ref '{0}' message listener with target method '{1}'.", targetType.FullName, NovaEngine.Utility.Text.ToString(methodTypeCodeInfo.Method));

                if (methodTypeCodeInfo.Opcode > 0)
                {
                    // if (reload) { obj.RemoveMessageListener(methodTypeCodeInfo.Opcode, methodTypeCodeInfo.Method); }

                    obj.AddMessageListener(methodTypeCodeInfo.Opcode, methodTypeCodeInfo.Method, true);
                }
                else
                {
                    // if (reload) { obj.RemoveMessageListener(methodTypeCodeInfo.MessageType, methodTypeCodeInfo.Method); }

                    obj.AddMessageListener(methodTypeCodeInfo.MessageType, methodTypeCodeInfo.Method, true);
                }
            }
        }
    }
}
