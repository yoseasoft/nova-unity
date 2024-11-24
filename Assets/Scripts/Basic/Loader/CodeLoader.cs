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
using SystemAssembly = System.Reflection.Assembly;

using SystemMemoryStream = System.IO.MemoryStream;

namespace GameEngine.Loader
{
    /// <summary>
    /// 程序集的分析处理类，对业务层载入的所有对象类进行统一加载及分析处理<br/>
    /// 该处理类主要通过反射实现对象初始化及清理流程中的一些模版配置管理
    /// </summary>
    public static partial class CodeLoader
    {
        /// <summary>
        /// 编码类型加载操作的回调句柄
        /// </summary>
        /// <param name="targetType">对象类型</param>
        /// <param name="codeInfo">类的结构信息</param>
        /// <param name="reload">重载状态标识</param>
        public delegate void OnCodeTypeLoadedHandler(SystemType targetType, GeneralCodeInfo codeInfo, bool reload);

        /// <summary>
        /// 编码类型清除操作的回调句柄
        /// </summary>
        public delegate void OnCleanupAllCodeTypesHandler();

        /// <summary>
        /// 程序集的版本记录容器
        /// </summary>
        private static readonly IDictionary<string, SystemAssembly> s_assemblyLibraryCaches = new Dictionary<string, SystemAssembly>();

        /// <summary>
        /// 程序集的类类型统计列表
        /// </summary>
        private static readonly IList<SystemType> s_assemblyClassTypes = new List<SystemType>();

        /// <summary>
        /// 对象类的编码类型加载回调句柄映射容器
        /// </summary>
        private static readonly IDictionary<SystemType, LinkedList<OnCodeTypeLoadedHandler>> s_codeTypeLoadedCallbacks = new Dictionary<SystemType, LinkedList<OnCodeTypeLoadedHandler>>();

        /// <summary>
        /// 程序集加载器的启动函数
        /// </summary>
        public static void Startup()
        {
            // 子模块初始化回调函数
            OnCodeLoaderSubmoduleInitCallback();
        }

        /// <summary>
        /// 程序集加载器的重启函数
        /// </summary>
        public static void Restart()
        {
            // throw new System.NotImplementedException();

            // 移除全部类类型统计信息
            RemoveAllAssemblyLibraries();
        }

        /// <summary>
        /// 程序集加载器的关闭函数
        /// </summary>
        public static void Shutdown()
        {
            // 移除全部外部监听回调句柄
            RemoveAllListenerLoadedCallbacks();

            // 移除全部类类型统计信息
            RemoveAllAssemblyLibraries();

            // 卸载全部实体配置数据信息
            UnloadAllConfigures();

            // 移除全部类反射操作回调句柄
            RemoveAllCodeTypeLoadedCallbacks();

            // 子模块清理回调函数
            OnCodeLoaderSubmoduleCleanupCallback();
        }

        /// <summary>
        /// 移除通过程序集加载的所有对象类型
        /// </summary>
        private static void RemoveAllAssemblyLoadTypes()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// 从指定的程序集加载对应的代码类
        /// </summary>
        /// <param name="assembly">程序集</param>
        /// <param name="reload">重载标识</param>
        public static void LoadFromAssembly(SystemAssembly assembly, bool reload)
        {
            // string assemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName().FullName;
            string assemblyName = assembly.FullName;
            if (s_assemblyLibraryCaches.TryGetValue(assemblyName, out SystemAssembly assemblyVersion))
            {
                Debugger.Warn("The target assembly '{0}' was already loaded, repeat load it failed.", assemblyName);
                return;
            }

            s_assemblyLibraryCaches.Add(assemblyName, assembly);

            SystemType[] allTypes = assembly.GetTypes();

            // LinkedList<OnCodeTypeLoadedHandler> handlers = null;
            // 用字典模式，在获取句柄列表的同时，对其因目标解析对象类型的不同而进行分类
            IDictionary<SystemType, IList<OnCodeTypeLoadedHandler>> handlers = null;
            for (int n = 0; null != allTypes && n < allTypes.Length; ++n)
            {
                SystemType type = allTypes[n];
                Debugger.Assert(false == s_assemblyClassTypes.Contains(type), "Invalid class type {0}.", type.FullName);
                s_assemblyClassTypes.Add(type);

                // 过滤忽略加载的目标对象类型
                if (false == IsLoadableClassType(type))
                {
                    Debugger.Info(LogGroupTag.CodeLoader, "The class type '{0}' was unloadable, ignore it with current loaded.", NovaEngine.Utility.Text.ToString(type));
                    continue;
                }

                Symboling.SymClass symClass = LoadSymClass(type, reload);
                // 加载对象类型的标记信息
                if (null == symClass)
                {
                    Debugger.Warn("Load class symbole from target type '{0}' failed.", NovaEngine.Utility.Text.ToString(type));
                    continue;
                }

                bool succ = LoadGeneralClass(symClass, reload);

                /*
                if (TryGetCodeTypeLoadedHandler(type, out handlers))
                {
                    GeneralCodeInfo info = null;
                    if (succ)
                    {
                        info = LookupGeneralCodeInfo(type, null);
                    }

                    foreach (OnCodeTypeLoadedHandler handler in handlers)
                    {
                        handler(type, info, reload);
                    }
                }
                */

                // 新版本支持同一个对象类进行多次不同类型的解析处理
                if (TryGetAllMathcedCodeTypeLoadedHandlerWithTargetClassType(type, out handlers))
                {
                    GeneralCodeInfo info = null;
                    foreach (KeyValuePair<SystemType, IList<OnCodeTypeLoadedHandler>> pair in handlers)
                    {
                        IEnumerator<OnCodeTypeLoadedHandler> e = pair.Value.GetEnumerator();
                        while (e.MoveNext())
                        {
                            OnCodeTypeLoadedHandler handler = e.Current;
                            if (succ)
                            {
                                // info = LookupGeneralCodeInfo(symClass, GetProcessRegisterClassTypeByHandler(handler));
                                info = LookupGeneralCodeInfo(symClass, pair.Key);
                            }

                            handler(type, info, reload);
                        }
                    }
                }

                // 解析进度的通知转发
                CallLoadClassProgress(assemblyName, type, n, allTypes.Length);
            }

            // 完成后的通知转发
            CallLoadAssemblyCompleted(assemblyName);
        }

        /// <summary>
        /// 检测目标对象类型是否为可加载的类型
        /// </summary>
        /// <param name="targetType">对象类型</param>
        /// <returns>若给定类型可加载则返回true，否则返回false</returns>
        private static bool IsLoadableClassType(SystemType targetType)
        {
            // 非对象类或接口类
            if (false == targetType.IsClass && false == targetType.IsInterface)
            {
                return false;
            }

            // 泛型类或属性类
            if (targetType.IsGenericType || targetType.IsSubclassOf(typeof(SystemAttribute)))
            {
                return false;
            }

            // 非公开类或内部类
            if (targetType.IsNotPublic || targetType.IsNested)
            {
                return false;
            }

            // 由编译器自动生成的对象类
            if (NovaEngine.Utility.Reflection.IsTypeOfCompilerGeneratedClass(targetType))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 检测当前的解析回调管理容器中是否存在指定类型对应的回调句柄
        /// </summary>
        /// <param name="targetType">对象类型</param>
        /// <returns>若存在给定类型对应的回调句柄则返回true，否则返回false</returns>
        private static bool IsCodeTypeLoadedCallbackExist(SystemType targetType)
        {
            LinkedList<OnCodeTypeLoadedHandler> handlers = null;
            if (TryGetCodeTypeLoadedHandler(targetType, out handlers) && handlers.Count > 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 通过指定的类型从加载回调管理容器中查找对应的回调句柄实例
        /// </summary>
        /// <param name="targetType">对象类型</param>
        /// <param name="handler">句柄列表</param>
        /// <returns>若查找句柄列表成功返回true，否则返回false</returns>
        private static bool TryGetCodeTypeLoadedHandler(SystemType targetType, out LinkedList<OnCodeTypeLoadedHandler> handler)
        {
            handler = null;

            IEnumerator<KeyValuePair<SystemType, LinkedList<OnCodeTypeLoadedHandler>>> e = s_codeTypeLoadedCallbacks.GetEnumerator();
            while (e.MoveNext())
            {
                if (e.Current.Key.IsSubclassOf(typeof(SystemAttribute)))
                {
                    // 属性类的绑定回调
                    IEnumerable<SystemAttribute> attrTypes = targetType.GetCustomAttributes();
                    foreach (SystemAttribute attrType in attrTypes)
                    {
                        if (e.Current.Key == attrType.GetType() || e.Current.Key.IsAssignableFrom(attrType.GetType()))
                        {
                            handler = e.Current.Value;
                            return true;
                        }
                    }
                }
                else
                {
                    // 对象类的绑定回调
                    if (e.Current.Key == targetType || e.Current.Key.IsAssignableFrom(targetType))
                    {
                        handler = e.Current.Value;
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 通过指定的类型从加载回调管理容器中查找对应的回调句柄实例<br/>
        /// 该函数与<see cref="GameEngine.CodeLoader.TryGetCodeTypeLoadedHandler(SystemType, out LinkedList{OnCodeTypeLoadedHandler})"/>函数的区别在于<br/>
        /// 它同时返回句柄对应的目标解析对象类的类型，您可以在调用句柄前用该类型直接进行一些前置的检测操作
        /// </summary>
        /// <param name="targetType">对象类型</param>
        /// <param name="handlers">句柄列表</param>
        /// <returns>若查找句柄列表成功返回true，否则返回false</returns>
        private static bool TryGetAllMathcedCodeTypeLoadedHandlerWithTargetClassType(SystemType targetType, out IDictionary<SystemType, IList<OnCodeTypeLoadedHandler>> handlers)
        {
            bool result = false;

            IDictionary<SystemType, IList<OnCodeTypeLoadedHandler>> dict = null;

            IEnumerator<KeyValuePair<SystemType, LinkedList<OnCodeTypeLoadedHandler>>> e = s_codeTypeLoadedCallbacks.GetEnumerator();
            while (e.MoveNext())
            {
                if (e.Current.Key.IsSubclassOf(typeof(SystemAttribute)))
                {
                    // 属性类的绑定回调
                    IEnumerable<SystemAttribute> attrTypes = targetType.GetCustomAttributes();
                    foreach (SystemAttribute attrType in attrTypes)
                    {
                        if (e.Current.Key == attrType.GetType() || e.Current.Key.IsAssignableFrom(attrType.GetType()))
                        {
                            if (null == dict) dict = new Dictionary<SystemType, IList<OnCodeTypeLoadedHandler>>();
                            Debugger.Assert(false == dict.ContainsKey(e.Current.Key), "Repeat added it failed.");

                            List<OnCodeTypeLoadedHandler> list = new List<OnCodeTypeLoadedHandler>();
                            list.AddRange(NovaEngine.Utility.Collection.ToArray<OnCodeTypeLoadedHandler>(e.Current.Value));
                            dict.Add(e.Current.Key, list);

                            break;
                        }
                    }
                }
                else
                {
                    // 对象类的绑定回调
                    if (e.Current.Key == targetType || e.Current.Key.IsAssignableFrom(targetType))
                    {
                        if (null == dict) dict = new Dictionary<SystemType, IList<OnCodeTypeLoadedHandler>>();
                        Debugger.Assert(false == dict.ContainsKey(e.Current.Key), "Repeat added it failed.");

                        List<OnCodeTypeLoadedHandler> list = new List<OnCodeTypeLoadedHandler>();
                        list.AddRange(NovaEngine.Utility.Collection.ToArray<OnCodeTypeLoadedHandler>(e.Current.Value));
                        dict.Add(e.Current.Key, list);
                    }
                }
            }

            if (null != dict && dict.Count > 0)
            {
                handlers = dict;
                result = true;
            }
            else
            {
                handlers = null;
            }

            return result;
        }

        #region 对象类型统计列表访问接口函数

        /// <summary>
        /// 通过指定的过滤条件从当前统计的类型列表中查找符合条件的类型实例
        /// </summary>
        /// <param name="func">过滤函数</param>
        /// <returns>若查找类型成功返回对应的列表，否则返回null</returns>
        public static IList<SystemType> FindClassTypesByFilterCondition(System.Func<SystemType, bool> func)
        {
            IList<SystemType> result = new List<SystemType>();

            for (int n = 0; n < s_assemblyClassTypes.Count; ++n)
            {
                SystemType targetType = s_assemblyClassTypes[n];
                if (func(targetType))
                {
                    result.Add(targetType);
                }
            }

            if (result.Count <= 0) { result = null; }

            return result;
        }

        /// <summary>
        /// 移除所有程序集及其内部的类类型统计信息
        /// </summary>
        private static void RemoveAllAssemblyLibraries()
        {
            UnloadAllSymClasses();

            s_assemblyLibraryCaches.Clear();
            s_assemblyClassTypes.Clear();
        }

        #endregion

        #region 对象类型绑定的回调句柄的注册/移除接口函数

        /// <summary>
        /// 添加新的类型及回调句柄实例到当前的加载回调管理容器中，若存在相同实例则不会重复添加
        /// </summary>
        /// <param name="targetType">对象类型</param>
        /// <param name="callback">回调句柄实例</param>
        public static void AddCodeTypeLoadedCallback(SystemType targetType, OnCodeTypeLoadedHandler callback)
        {
            LinkedList<OnCodeTypeLoadedHandler> handlers = null;
            if (false == TryGetCodeTypeLoadedHandler(targetType, out handlers))
            {
                handlers = new LinkedList<OnCodeTypeLoadedHandler>();
                handlers.AddLast(callback);

                s_codeTypeLoadedCallbacks.Add(targetType, handlers);
                return;
            }

            if (handlers.Contains(callback))
            {
                Debugger.Warn("The class '{0}' reflect code type loaded callback was already existed, repeat added it failed.", targetType.FullName);
                return;
            }

            handlers.AddLast(callback);
        }

        /// <summary>
        /// 从当前加载回调管理容器中移除指定类型的回调句柄实例
        /// </summary>
        /// <param name="targetType">对象类型</param>
        /// <param name="callback">回调句柄实例</param>
        public static void RemoveCodeTypeLoadedCallback(SystemType targetType, OnCodeTypeLoadedHandler callback)
        {
            LinkedList<OnCodeTypeLoadedHandler> handlers = null;
            if (false == TryGetCodeTypeLoadedHandler(targetType, out handlers))
            {
                Debugger.Warn("Could not found any load callback from this type '{0}', remove it failed.", targetType.FullName);
                return;
            }

            handlers.Remove(callback);
            if (handlers.Count <= 0)
            {
                s_codeTypeLoadedCallbacks.Remove(targetType);
            }
        }

        /// <summary>
        /// 移除当前解析回调管理容器中已注册的全部句柄实例
        /// </summary>
        private static void RemoveAllCodeTypeLoadedCallbacks()
        {
            s_codeTypeLoadedCallbacks.Clear();
        }

        #endregion

        #region 对象类型的配置数据加载/卸载接口函数

        /// <summary>
        /// 从指定的数据流中加载对象类型配置信息<br/>
        /// 需要注意的是，必须在加载程序集之前，加载完成所有的配置数据，否则将导致程序集中的类型解析时，无法抽取到正确配置信息
        /// </summary>
        /// <param name="buffer">数据流</param>
        /// <param name="offset">数据偏移</param>
        /// <param name="length">数据长度</param>
        public static void LoadFromConfigure(byte[] buffer, int offset, int length)
        {
            LoadGeneralConfigure(buffer, offset, length);
        }

        /// <summary>
        /// 从指定的数据流中加载对象类型配置信息<br/>
        /// 需要注意的是，必须在加载程序集之前，加载完成所有的配置数据，否则将导致程序集中的类型解析时，无法抽取到正确配置信息
        /// </summary>
        /// <param name="memoryStream">数据流</param>
        public static void LoadFromConfigure(SystemMemoryStream memoryStream)
        {
            LoadGeneralConfigure(memoryStream);
        }

        /// <summary>
        /// 卸载当前所有解析登记的配置数据对象实例<br/>
        /// 请注意，该接口不建议由用户自定调用，因为这需要充分了解代码加载的流程<br/>
        /// 并在合适的节点进行调度，否则将导致不可预知的问题
        /// </summary>
        public static void UnloadAllConfigures()
        {
            UnloadAllConfigureContents();
        }

        #endregion
    }
}
