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
using SystemAttribute = System.Attribute;
using SystemDelegate = System.Delegate;
using SystemMethodInfo = System.Reflection.MethodInfo;
using SystemBindingFlags = System.Reflection.BindingFlags;

namespace GameEngine
{
    /// <summary>
    /// 控制器对象的抽象基类，对所有的控制器类型进行标准接口函数及流程的封装
    /// </summary>
    public abstract class BaseController<T> : NovaEngine.Singleton<T>, IController where T : class, new()
    {
        /// <summary>
        /// 控制器子模块行为流程回调的缓存队列
        /// </summary>
        private IDictionary<SystemType, SystemDelegate> m_cachedSubmoduleBehaviourCallbacks = null;

        /// <summary>
        /// 控制器对象初始化通知接口函数
        /// </summary>
        protected override sealed void Initialize()
        {
            // 初始化子模块行为流程缓存队列
            m_cachedSubmoduleBehaviourCallbacks = new Dictionary<SystemType, SystemDelegate>();

            OnInitialize();

            // 子模块初始化回调
            OnSubmoduleInitCallback();
        }

        /// <summary>
        /// 控制器对象清理通知接口函数
        /// </summary>
        protected override sealed void Cleanup()
        {
            // 子模块清理回调
            OnSubmoduleCleanupCallback();

            OnCleanup();

            // 清理子模块行为流程缓存队列
            m_cachedSubmoduleBehaviourCallbacks.Clear();
            m_cachedSubmoduleBehaviourCallbacks = null;
        }

        /// <summary>
        /// 控制器对象初始化回调函数
        /// </summary>
        protected abstract void OnInitialize();

        /// <summary>
        /// 控制器对象清理回调函数
        /// </summary>
        protected abstract void OnCleanup();

        /// <summary>
        /// 控制器对象刷新调度函数接口
        /// </summary>
        public void Update()
        {
            OnUpdate();

            // 子模块刷新回调
            OnSubmoduleUpdateCallback();
        }

        /// <summary>
        /// 控制器对象后置刷新调度函数接口
        /// </summary>
        public void LateUpdate()
        {
            OnLateUpdate();

            // 子模块后置刷新回调
            OnSubmoduleLateUpdateCallback();
        }

        /// <summary>
        /// 控制器对象倾泻调度函数接口
        /// </summary>
        public void Dump()
        {
            OnDump();

            // 子模块倾泻回调
            OnSubmoduleDumpCallback();
        }

        /// <summary>
        /// 控制器对象刷新回调函数
        /// </summary>
        protected abstract void OnUpdate();

        /// <summary>
        /// 控制器对象后置刷新回调函数
        /// </summary>
        protected abstract void OnLateUpdate();

        /// <summary>
        /// 控制器对象倾泻回调函数
        /// </summary>
        protected abstract void OnDump();

        /// <summary>
        /// 控制器对象子模块初始化回调处理接口函数
        /// </summary>
        private void OnSubmoduleInitCallback()
        {
            OnSubmoduleActionCallbackOfTargetAttribute(typeof(OnControllerSubmoduleInitCallbackAttribute));
        }

        /// <summary>
        /// 控制器对象子模块清理回调处理接口函数
        /// </summary>
        private void OnSubmoduleCleanupCallback()
        {
            OnSubmoduleActionCallbackOfTargetAttribute(typeof(OnControllerSubmoduleCleanupCallbackAttribute));
        }

        /// <summary>
        /// 控制器对象子模块刷新回调处理接口函数
        /// </summary>
        private void OnSubmoduleUpdateCallback()
        {
            OnSubmoduleActionCallbackOfTargetAttribute(typeof(OnControllerSubmoduleUpdateCallbackAttribute));
        }

        /// <summary>
        /// 控制器对象子模块后置刷新回调处理接口函数
        /// </summary>
        private void OnSubmoduleLateUpdateCallback()
        {
            OnSubmoduleActionCallbackOfTargetAttribute(typeof(OnControllerSubmoduleLateUpdateCallbackAttribute));
        }

        /// <summary>
        /// 控制器对象子模块倾泻回调处理接口函数
        /// </summary>
        private void OnSubmoduleDumpCallback()
        {
            OnSubmoduleActionCallbackOfTargetAttribute(typeof(OnControllerSubmoduleDumpCallbackAttribute));
        }

        /// <summary>
        /// 控制器对象子模块指定类型的回调函数触发处理接口
        /// </summary>
        /// <param name="attrType">属性类型</param>
        private void OnSubmoduleActionCallbackOfTargetAttribute(SystemType attrType)
        {
            SystemDelegate callback;
            if (TryGetSubmoduleBehaviourCallback(attrType, out callback))
            {
                callback.DynamicInvoke();
            }
        }

        private bool TryGetSubmoduleBehaviourCallback(SystemType targetType, out SystemDelegate callback)
        {
            SystemDelegate handler;
            if (m_cachedSubmoduleBehaviourCallbacks.TryGetValue(targetType, out handler))
            {
                callback = handler;
                return null == callback ? false : true;
            }

            IList<SystemDelegate> list = new List<SystemDelegate>();
            SystemType classType = GetType();
            SystemMethodInfo[] methods = classType.GetMethods(SystemBindingFlags.Public | SystemBindingFlags.NonPublic | SystemBindingFlags.Instance);
            for (int n = 0; n < methods.Length; ++n)
            {
                SystemMethodInfo method = methods[n];
                SystemAttribute attr = method.GetCustomAttribute(targetType);
                if (null != attr)
                {
                    SystemDelegate c = method.CreateDelegate(typeof(NovaEngine.Definition.Delegate.EmptyFunctionHandler), this);
                    list.Add(c);
                }
            }

            // 先重置回调
            callback = null;

            if (list.Count > 0)
            {
                for (int n = 0; n < list.Count; ++n)
                {
                    if (null == callback)
                    {
                        callback = list[n];
                    }
                    else
                    {
                        callback = SystemDelegate.Combine(list[n], callback);
                    }
                }

                m_cachedSubmoduleBehaviourCallbacks.Add(targetType, callback);
                return true;
            }

            m_cachedSubmoduleBehaviourCallbacks.Add(targetType, callback);
            return false;
        }
    }
}
