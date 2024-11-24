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
using SystemDelegate = System.Delegate;
using SystemAttribute = System.Attribute;
using SystemAttributeUsageAttribute = System.AttributeUsageAttribute;
using SystemAttributeTargets = System.AttributeTargets;
using SystemMethodInfo = System.Reflection.MethodInfo;
using SystemBindingFlags = System.Reflection.BindingFlags;
using SystemStringBuilder = System.Text.StringBuilder;

namespace GameEngine.Loader
{
    /// <summary>
    /// 消息事件类的结构信息
    /// </summary>
    public class EventCodeInfo : GeneralCodeInfo
    {
        /// <summary>
        /// 消息事件类的类型标识
        /// </summary>
        protected SystemType m_classType;

        public SystemType ClassType { get { return m_classType; } internal set { m_classType = value; } }

        public override string ToString()
        {
            SystemStringBuilder sb = new SystemStringBuilder();
            sb.Append("{ ");
            sb.AppendFormat("Class = {0}, ", m_classType.FullName);
            sb.Append("}");
            return sb.ToString();
        }
    }

    /// <summary>
    /// 消息事件分发调度对象的分析处理类，对业务层载入的所有消息事件调度类进行统一加载及分析处理
    /// </summary>
    internal static partial class EventCodeLoader
    {
        /// <summary>
        /// 加载消息事件调度类相关回调函数的管理容器
        /// </summary>
        private static IDictionary<SystemType, SystemDelegate> s_eventClassLoadCallbacks = new Dictionary<SystemType, SystemDelegate>();
        /// <summary>
        /// 清理消息事件调度类相关回调函数的管理容器
        /// </summary>
        private static IDictionary<SystemType, SystemDelegate> s_eventClassCleanupCallbacks = new Dictionary<SystemType, SystemDelegate>();
        /// <summary>
        /// 查找消息事件调度类结构信息相关回调函数的管理容器
        /// </summary>
        private static IDictionary<SystemType, SystemDelegate> s_eventCodeInfoLookupCallbacks = new Dictionary<SystemType, SystemDelegate>();

        /// <summary>
        /// 加载消息事件调度类相关函数的属性定义
        /// </summary>
        [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = false, Inherited = false)]
        private class OnEventClassLoadOfTargetAttribute : OnCodeLoaderClassLoadOfTargetAttribute
        {
            public OnEventClassLoadOfTargetAttribute(SystemType classType) : base(classType) { }
        }

        /// <summary>
        /// 清理消息事件调度类相关函数的属性定义
        /// </summary>
        [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = false, Inherited = false)]
        private class OnEventClassCleanupOfTargetAttribute : OnCodeLoaderClassCleanupOfTargetAttribute
        {
            public OnEventClassCleanupOfTargetAttribute(SystemType classType) : base(classType) { }
        }

        /// <summary>
        /// 查找消息事件调度类对应结构信息相关函数的属性定义
        /// </summary>
        [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = false, Inherited = false)]
        private class OnEventCodeInfoLookupOfTargetAttribute : OnCodeLoaderClassLookupOfTargetAttribute
        {
            public OnEventCodeInfoLookupOfTargetAttribute(SystemType classType) : base (classType) { }
        }

        /// <summary>
        /// 初始化针对所有消息事件调度类声明的全部绑定回调接口
        /// </summary>
        [CodeLoader.OnGeneralCodeLoaderInit]
        private static void InitAllEventClassLoadingCallbacks()
        {
            SystemType classType = typeof(EventCodeLoader);
            SystemMethodInfo[] methods = classType.GetMethods(SystemBindingFlags.Public | SystemBindingFlags.NonPublic | SystemBindingFlags.Static);
            for (int n = 0; n < methods.Length; ++n)
            {
                SystemMethodInfo method = methods[n];
                IEnumerable<SystemAttribute> e = method.GetCustomAttributes();
                foreach (SystemAttribute attr in e)
                {
                    SystemType attrType = attr.GetType();
                    if (typeof(OnEventClassLoadOfTargetAttribute) == attrType)
                    {
                        OnEventClassLoadOfTargetAttribute _attr = (OnEventClassLoadOfTargetAttribute) attr;

                        Debugger.Assert(!s_eventClassLoadCallbacks.ContainsKey(_attr.ClassType), "Invalid event class load type");
                        s_eventClassLoadCallbacks.Add(_attr.ClassType, method.CreateDelegate(typeof(CodeLoader.OnGeneralCodeLoaderLoadHandler)));
                    }
                    else if (typeof(OnEventClassCleanupOfTargetAttribute) == attrType)
                    {
                        OnEventClassCleanupOfTargetAttribute _attr = (OnEventClassCleanupOfTargetAttribute) attr;

                        Debugger.Assert(!s_eventClassCleanupCallbacks.ContainsKey(_attr.ClassType), "Invalid event class cleanup type");
                        s_eventClassCleanupCallbacks.Add(_attr.ClassType, method.CreateDelegate(typeof(CodeLoader.OnCleanupAllGeneralCodeLoaderHandler)));
                    }
                    else if (typeof(OnEventCodeInfoLookupOfTargetAttribute) == attrType)
                    {
                        OnEventCodeInfoLookupOfTargetAttribute _attr = (OnEventCodeInfoLookupOfTargetAttribute) attr;

                        Debugger.Assert(!s_eventCodeInfoLookupCallbacks.ContainsKey(_attr.ClassType), "Invalid event class lookup type");
                        s_eventCodeInfoLookupCallbacks.Add(_attr.ClassType, method.CreateDelegate(typeof(CodeLoader.OnGeneralCodeLoaderLookupHandler)));
                    }
                }
            }
        }

        /// <summary>
        /// 清理针对所有消息事件调度类声明的全部绑定回调接口
        /// </summary>
        [CodeLoader.OnGeneralCodeLoaderCleanup]
        private static void CleanupAllEventClassLoadingCallbacks()
        {
            IEnumerator<KeyValuePair<SystemType, SystemDelegate>> e = s_eventClassCleanupCallbacks.GetEnumerator();
            while (e.MoveNext())
            {
                CodeLoader.OnCleanupAllGeneralCodeLoaderHandler handler = e.Current.Value as CodeLoader.OnCleanupAllGeneralCodeLoaderHandler;
                Debugger.Assert(null != handler, "Invalid event class cleanup handler.");

                handler.Invoke();
            }

            s_eventClassLoadCallbacks.Clear();
            s_eventClassCleanupCallbacks.Clear();
            s_eventCodeInfoLookupCallbacks.Clear();
        }

        /// <summary>
        /// 检测消息事件调度类指定的类型是否匹配当前加载器
        /// </summary>
        /// <param name="symClass">对象标记类型</param>
        /// <param name="filterType">过滤对象类型</param>
        /// <returns>若给定类型满足匹配规则则返回true，否则返回false</returns>
        [CodeLoader.OnGeneralCodeLoaderMatch]
        private static bool IsEventClassMatched(Symboling.SymClass symClass, SystemType filterType)
        {
            // 存在过滤类型，则直接对比过滤类型即可
            if (null != filterType)
            {
                return IsEventClassCallbackExist(filterType);
            }

            IList<SystemAttribute> attrs = symClass.Attributes;
            for (int n = 0; null != attrs && n < attrs.Count; ++n)
            {
                SystemAttribute attr = attrs[n];
                if (IsEventClassCallbackExist(attr.GetType()))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 加载消息事件调度类指定的类型
        /// </summary>
        /// <param name="symClass">对象标记类型</param>
        /// <param name="reload">重载状态标识</param>
        /// <returns>若存在给定类型属性消息事件调度类则返回对应处理结果，否则返回false</returns>
        [CodeLoader.OnGeneralCodeLoaderLoad]
        private static bool LoadEventClass(Symboling.SymClass symClass, bool reload)
        {
            SystemDelegate callback = null;

            IList<SystemAttribute> attrs = symClass.Attributes;
            for (int n = 0; null != attrs && n < attrs.Count; ++n)
            {
                SystemAttribute attr = attrs[n];
                if (TryGetEventClassCallbackForTargetContainer(attr.GetType(), out callback, s_eventClassLoadCallbacks))
                {
                    CodeLoader.OnGeneralCodeLoaderLoadHandler handler = callback as CodeLoader.OnGeneralCodeLoaderLoadHandler;
                    Debugger.Assert(null != handler, "Invalid event class load handler.");
                    return handler.Invoke(symClass, reload);
                }
            }

            return false;
        }

        /// <summary>
        /// 查找消息事件调度类指定的类型对应的结构信息
        /// </summary>
        /// <param name="symClass">对象标记类型</param>
        /// <returns>返回类型对应的结构信息</returns>
        [CodeLoader.OnGeneralCodeLoaderLookup]
        private static GeneralCodeInfo LookupEventCodeInfo(Symboling.SymClass symClass)
        {
            SystemDelegate callback = null;

            IList<SystemAttribute> attrs = symClass.Attributes;
            for (int n = 0; null != attrs && n < attrs.Count; ++n)
            {
                SystemAttribute attr = attrs[n];
                if (TryGetEventClassCallbackForTargetContainer(attr.GetType(), out callback, s_eventCodeInfoLookupCallbacks))
                {
                    CodeLoader.OnGeneralCodeLoaderLookupHandler handler = callback as CodeLoader.OnGeneralCodeLoaderLookupHandler;
                    Debugger.Assert(null != handler, "Invalid event class lookup handler.");
                    return handler.Invoke(symClass);
                }
            }

            return null;
        }

        /// <summary>
        /// 检测当前的回调管理容器中是否存在指定类型的消息事件调度回调
        /// </summary>
        /// <param name="targetType">对象类型</param>
        /// <returns>若存在给定类型对应的回调句柄则返回true，否则返回false</returns>
        private static bool IsEventClassCallbackExist(SystemType targetType)
        {
            IEnumerator<KeyValuePair<SystemType, SystemDelegate>> e = s_eventClassLoadCallbacks.GetEnumerator();
            while (e.MoveNext())
            {
                // 这里的类型为属性定义的类型，因此直接作相等比较即可
                if (e.Current.Key == targetType)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 通过指定的类型从消息事件调度类的回调管理容器中查找对应的回调句柄实例
        /// </summary>
        /// <param name="targetType">对象类型</param>
        /// <param name="callback">回调句柄</param>
        /// <param name="container">句柄列表容器</param>
        /// <returns>返回通过给定类型查找的回调句柄实例，若不存在则返回null</returns>
        private static bool TryGetEventClassCallbackForTargetContainer(SystemType targetType, out SystemDelegate callback, IDictionary<SystemType, SystemDelegate> container)
        {
            callback = null;

            IEnumerator<KeyValuePair<SystemType, SystemDelegate>> e = container.GetEnumerator();
            while (e.MoveNext())
            {
                // 这里的类型为属性定义的类型，因此直接作相等比较即可
                if (e.Current.Key == targetType)
                {
                    callback = e.Current.Value;
                    return true;
                }
            }

            return false;
        }
    }
}
