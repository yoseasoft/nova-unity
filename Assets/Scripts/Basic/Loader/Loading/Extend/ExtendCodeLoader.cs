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
    /// 扩展定义类的结构信息
    /// </summary>
    public class ExtendCodeInfo : GeneralCodeInfo
    {
        /// <summary>
        /// 扩展定义类的类型标识
        /// </summary>
        protected SystemType m_classType;

        public SystemType ClassType { get { return m_classType; } set { m_classType = value; } }

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
    /// 程序集中扩展定义对象的分析处理类，对业务层载入的所有扩展定义对象类进行统一加载及分析处理
    /// </summary>
    internal static partial class ExtendCodeLoader
    {
        /// <summary>
        /// 加载扩展定义类相关回调函数的管理容器
        /// </summary>
        private static IDictionary<SystemType, SystemDelegate> s_extendClassLoadCallbacks = new Dictionary<SystemType, SystemDelegate>();
        /// <summary>
        /// 清理扩展定义类相关回调函数的管理容器
        /// </summary>
        private static IDictionary<SystemType, SystemDelegate> s_extendClassCleanupCallbacks = new Dictionary<SystemType, SystemDelegate>();
        /// <summary>
        /// 查找扩展定义类结构信息相关回调函数的管理容器
        /// </summary>
        private static IDictionary<SystemType, SystemDelegate> s_extendCodeInfoLookupCallbacks = new Dictionary<SystemType, SystemDelegate>();

        /// <summary>
        /// 加载扩展定义类相关函数的属性定义
        /// </summary>
        [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = false, Inherited = false)]
        private class OnExtendClassLoadOfTargetAttribute : OnCodeLoaderClassLoadOfTargetAttribute
        {
            public OnExtendClassLoadOfTargetAttribute(SystemType classType) : base(classType) { }
        }

        /// <summary>
        /// 清理扩展定义类相关函数的属性定义
        /// </summary>
        [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = false, Inherited = false)]
        private class OnExtendClassCleanupOfTargetAttribute : OnCodeLoaderClassCleanupOfTargetAttribute
        {
            public OnExtendClassCleanupOfTargetAttribute(SystemType classType) : base(classType) { }
        }

        /// <summary>
        /// 查找扩展定义类对应结构信息相关函数的属性定义
        /// </summary>
        [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = false, Inherited = false)]
        private class OnExtendCodeInfoLookupOfTargetAttribute : OnCodeLoaderClassLookupOfTargetAttribute
        {
            public OnExtendCodeInfoLookupOfTargetAttribute(SystemType classType) : base(classType) { }
        }

        /// <summary>
        /// 初始化针对所有扩展定义类声明的全部绑定回调接口
        /// </summary>
        [CodeLoader.OnGeneralCodeLoaderInit]
        private static void InitAllExtendClassLoadingCallbacks()
        {
            SystemType classType = typeof(ExtendCodeLoader);
            SystemMethodInfo[] methods = classType.GetMethods(SystemBindingFlags.Public | SystemBindingFlags.NonPublic | SystemBindingFlags.Static);
            for (int n = 0; n < methods.Length; ++n)
            {
                SystemMethodInfo method = methods[n];
                IEnumerable<SystemAttribute> e = method.GetCustomAttributes();
                foreach (SystemAttribute attr in e)
                {
                    SystemType attrType = attr.GetType();
                    if (typeof(OnExtendClassLoadOfTargetAttribute) == attrType)
                    {
                        OnExtendClassLoadOfTargetAttribute _attr = (OnExtendClassLoadOfTargetAttribute) attr;

                        Debugger.Assert(!s_extendClassLoadCallbacks.ContainsKey(_attr.ClassType), "Invalid extend class load type");
                        s_extendClassLoadCallbacks.Add(_attr.ClassType, method.CreateDelegate(typeof(CodeLoader.OnGeneralCodeLoaderLoadHandler)));
                    }
                    else if (typeof(OnExtendClassCleanupOfTargetAttribute) == attrType)
                    {
                        OnExtendClassCleanupOfTargetAttribute _attr = (OnExtendClassCleanupOfTargetAttribute) attr;

                        Debugger.Assert(!s_extendClassCleanupCallbacks.ContainsKey(_attr.ClassType), "Invalid extend class cleanup type");
                        s_extendClassCleanupCallbacks.Add(_attr.ClassType, method.CreateDelegate(typeof(CodeLoader.OnCleanupAllGeneralCodeLoaderHandler)));
                    }
                    else if (typeof(OnExtendCodeInfoLookupOfTargetAttribute) == attrType)
                    {
                        OnExtendCodeInfoLookupOfTargetAttribute _attr = (OnExtendCodeInfoLookupOfTargetAttribute) attr;

                        Debugger.Assert(!s_extendCodeInfoLookupCallbacks.ContainsKey(_attr.ClassType), "Invalid extend class lookup type");
                        s_extendCodeInfoLookupCallbacks.Add(_attr.ClassType, method.CreateDelegate(typeof(CodeLoader.OnGeneralCodeLoaderLookupHandler)));
                    }
                }
            }
        }

        /// <summary>
        /// 清理针对所有扩展定义类声明的全部绑定回调接口
        /// </summary>
        [CodeLoader.OnGeneralCodeLoaderCleanup]
        private static void CleanupAllExtendClassLoadingCallbacks()
        {
            IEnumerator<KeyValuePair<SystemType, SystemDelegate>> e = s_extendClassCleanupCallbacks.GetEnumerator();
            while (e.MoveNext())
            {
                CodeLoader.OnCleanupAllGeneralCodeLoaderHandler handler = e.Current.Value as CodeLoader.OnCleanupAllGeneralCodeLoaderHandler;
                Debugger.Assert(null != handler, "Invalid extend class cleanup handler.");

                handler.Invoke();
            }

            s_extendClassLoadCallbacks.Clear();
            s_extendClassCleanupCallbacks.Clear();
            s_extendCodeInfoLookupCallbacks.Clear();
        }

        /// <summary>
        /// 检测扩展定义类指定的类型是否匹配当前加载器
        /// </summary>
        /// <param name="symClass">对象标记类型</param>
        /// <param name="filterType">过滤对象类型</param>
        /// <returns>若给定类型满足匹配规则则返回true，否则返回false</returns>
        [CodeLoader.OnGeneralCodeLoaderMatch]
        private static bool IsExtendClassMatched(Symboling.SymClass symClass, SystemType filterType)
        {
            // 存在过滤类型，则直接对比过滤类型即可
            if (null != filterType)
            {
                return IsExtendClassCallbackExist(filterType);
            }

            // 扩展类必须为静态类
            if (false == symClass.IsStatic)
            {
                return false;
            }

            IList<SystemAttribute> attrs = symClass.Attributes;
            for (int n = 0; null != attrs && n < attrs.Count; ++n)
            {
                SystemAttribute attr = attrs[n];
                if (IsExtendClassCallbackExist(attr.GetType()))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 加载扩展定义类指定的类型
        /// </summary>
        /// <param name="symClass">对象标记类型</param>
        /// <param name="reload">重载状态标识</param>
        /// <returns>若存在给定类型属性扩展定义类则返回对应处理结果，否则返回false</returns>
        [CodeLoader.OnGeneralCodeLoaderLoad]
        private static bool LoadExtendClass(Symboling.SymClass symClass, bool reload)
        {
            SystemDelegate callback = null;

            IList<SystemAttribute> attrs = symClass.Attributes;
            for (int n = 0; null != attrs && n < attrs.Count; ++n)
            {
                SystemAttribute attr = attrs[n];
                if (TryGetExtendClassCallbackForTargetContainer(attr.GetType(), out callback, s_extendClassLoadCallbacks))
                {
                    CodeLoader.OnGeneralCodeLoaderLoadHandler handler = callback as CodeLoader.OnGeneralCodeLoaderLoadHandler;
                    Debugger.Assert(null != handler, "Invalid extend class load handler.");
                    return handler.Invoke(symClass, reload);
                }
            }

            return false;
        }

        /// <summary>
        /// 查找扩展定义类指定的类型对应的结构信息
        /// </summary>
        /// <param name="symClass">对象标记类型</param>
        /// <returns>返回类型对应的结构信息</returns>
        [CodeLoader.OnGeneralCodeLoaderLookup]
        private static GeneralCodeInfo LookupExtendCodeInfo(Symboling.SymClass symClass)
        {
            SystemDelegate callback = null;

            IList<SystemAttribute> attrs = symClass.Attributes;
            for (int n = 0; null != attrs && n < attrs.Count; ++n)
            {
                SystemAttribute attr = attrs[n];
                if (TryGetExtendClassCallbackForTargetContainer(attr.GetType(), out callback, s_extendCodeInfoLookupCallbacks))
                {
                    CodeLoader.OnGeneralCodeLoaderLookupHandler handler = callback as CodeLoader.OnGeneralCodeLoaderLookupHandler;
                    Debugger.Assert(null != handler, "Invalid extend class lookup handler.");
                    return handler.Invoke(symClass);
                }
            }

            return null;
        }

        /// <summary>
        /// 检测当前的回调管理容器中是否存在指定类型的扩展定义回调
        /// </summary>
        /// <param name="targetType">对象类型</param>
        /// <returns>若存在给定类型对应的回调句柄则返回true，否则返回false</returns>
        private static bool IsExtendClassCallbackExist(SystemType targetType)
        {
            IEnumerator<KeyValuePair<SystemType, SystemDelegate>> e = s_extendClassLoadCallbacks.GetEnumerator();
            while (e.MoveNext())
            {
                // 这里的属性类型允许继承，因此不能直接作相等比较
                if (e.Current.Key.IsAssignableFrom(targetType))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 通过指定的类型从扩展定义类的回调管理容器中查找对应的回调句柄实例
        /// </summary>
        /// <param name="targetType">对象类型</param>
        /// <param name="callback">回调句柄</param>
        /// <param name="container">句柄列表容器</param>
        /// <returns>返回通过给定类型查找的回调句柄实例，若不存在则返回null</returns>
        private static bool TryGetExtendClassCallbackForTargetContainer(SystemType targetType, out SystemDelegate callback, IDictionary<SystemType, SystemDelegate> container)
        {
            callback = null;

            IEnumerator<KeyValuePair<SystemType, SystemDelegate>> e = container.GetEnumerator();
            while (e.MoveNext())
            {
                // 这里的属性类型允许继承，因此不能直接作相等比较
                if (e.Current.Key.IsAssignableFrom(targetType))
                {
                    callback = e.Current.Value;
                    return true;
                }
            }

            return false;
        }
    }
}
