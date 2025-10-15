/// <summary>
/// 2025-01-02 Game Framework Code By Hurley
/// </summary>

using System.Collections.Generic;

using SystemType = System.Type;
using SystemDelegate = System.Delegate;
using SystemMethodInfo = System.Reflection.MethodInfo;
using SystemBindingFlags = System.Reflection.BindingFlags;

namespace Game
{
    /// <summary>
    /// 程序的世界管理容器入口封装对象类，提供底层调度业务层的总入口
    /// </summary>
    public static partial class GameWorld
    {
        /// <summary>
        /// 观察者对象实例存储容器
        /// </summary>
        private static IList<IWorldObserver> _worldObservers = null;
        /// <summary>
        /// 观察者对象类型及其绑定函数的存储容器
        /// </summary>
        private static IDictionary<SystemType, IDictionary<string, NovaEngine.Definition.Delegate.EmptyFunctionHandler>> _worldObserverTypes = null;
        /// <summary>
        /// 观察接口与通知类型的映射容器
        /// </summary>
        private static IDictionary<string, NovaEngine.Definition.Delegate.EmptyFunctionHandler> _worldObserverNamesMapping = null;

        /// <summary>
        /// 初始化观察接口上下文信息
        /// </summary>
        private static void InitWorldObserverContext()
        {
            _worldObservers = new List<IWorldObserver>();
            _worldObserverTypes = new Dictionary<SystemType, IDictionary<string, NovaEngine.Definition.Delegate.EmptyFunctionHandler>>();
            _worldObserverNamesMapping = new Dictionary<string, NovaEngine.Definition.Delegate.EmptyFunctionHandler>();

            //SystemArray enumArray = SystemEnum.GetValues(typeof(NovaEngine.Application.ProtocolType));
            //for (int n = 0; n < enumArray.Length; ++n)
            //{
            //    NovaEngine.Application.ProtocolType protocolType = (NovaEngine.Application.ProtocolType) enumArray.GetValue(n);
            //    if (protocolType < NovaEngine.Application.ProtocolType.Startup || protocolType > NovaEngine.Application.ProtocolType.LateExecute) { continue; }
            //}

            _worldObserverNamesMapping.Add(NovaEngine.Application.ProtocolType.Startup.ToString(), _startupCallbacks);
            _worldObserverNamesMapping.Add(NovaEngine.Application.ProtocolType.Shutdown.ToString(), _shutdownCallbacks);
            _worldObserverNamesMapping.Add(NovaEngine.Application.ProtocolType.FixedExecute.ToString(), _fixedExecuteCallbacks);
            _worldObserverNamesMapping.Add(NovaEngine.Application.ProtocolType.Execute.ToString(), _executeCallbacks);
            _worldObserverNamesMapping.Add(NovaEngine.Application.ProtocolType.LateExecute.ToString(), _lateExecuteCallbacks);
            _worldObserverNamesMapping.Add(NovaEngine.Application.ProtocolType.FixedUpdate.ToString(), _fixedUpdateCallbacks);
            _worldObserverNamesMapping.Add(NovaEngine.Application.ProtocolType.Update.ToString(), _updateCallbacks);
            _worldObserverNamesMapping.Add(NovaEngine.Application.ProtocolType.LateUpdate.ToString(), _lateUpdateCallbacks);
        }

        /// <summary>
        /// 清理观察接口上下文信息
        /// </summary>
        private static void CleanupWorldObserverContext()
        {
            // 注销所有的观察对象实例
            UnregisterAllWorldObservers();

            _worldObservers.Clear();
            _worldObservers = null;

            _worldObserverTypes.Clear();
            _worldObserverTypes = null;

            _worldObserverNamesMapping.Clear();
            _worldObserverNamesMapping = null;
        }

        /// <summary>
        /// 注册世界通知的观察对象实例
        /// </summary>
        /// <param name="observer">观察对象实例</param>
        internal static void RegisterWorldObserver(IWorldObserver observer)
        {
            Debugger.Assert(null != observer, "The world observer must be non-null.");

            if (_worldObservers.Contains(observer))
            {
                Debugger.Warn("The world observer instance was already exist from type '{%t}', repeat added it failed.", observer);
                return;
            }

            _worldObservers.Add(observer);

            _startupCallbacks += observer.Startup;
            _shutdownCallbacks += observer.Shutdown;
            _fixedExecuteCallbacks += observer.FixedExecute;
            _executeCallbacks += observer.Execute;
            _lateExecuteCallbacks += observer.LateExecute;
            _fixedUpdateCallbacks += observer.FixedUpdate;
            _updateCallbacks += observer.Update;
            _lateUpdateCallbacks += observer.LateUpdate;
        }

        /// <summary>
        /// 注销世界通知的观察对象实例
        /// </summary>
        /// <param name="observer">观察对象实例</param>
        internal static void UnregisterWorldObserver(IWorldObserver observer)
        {
            Debugger.Assert(null != observer, "The world observer must be non-null.");

            if (null == _worldObservers || false == _worldObservers.Contains(observer))
            {
                Debugger.Warn("Could not found any world observer instance with class type '{%t}', removed it failed.", observer);
                return;
            }

            _startupCallbacks -= observer.Startup;
            _shutdownCallbacks -= observer.Shutdown;
            _fixedExecuteCallbacks -= observer.FixedExecute;
            _executeCallbacks -= observer.Execute;
            _lateExecuteCallbacks -= observer.LateExecute;
            _fixedUpdateCallbacks -= observer.FixedUpdate;
            _updateCallbacks -= observer.Update;
            _lateUpdateCallbacks -= observer.LateUpdate;

            _worldObservers.Remove(observer);
        }

        /// <summary>
        /// 注销当前世界通知的全部观察对象实例
        /// </summary>
        private static void UnregisterAllWorldObservers()
        {
            while (_worldObservers.Count > 0)
            {
                IWorldObserver observer = _worldObservers[0];
                UnregisterWorldObserver(observer);
            }

            IList<SystemType> observerTypes = NovaEngine.Utility.Collection.ToList<SystemType>(_worldObserverTypes.Keys);
            for (int n = 0; n < observerTypes.Count; ++n)
            {
                UnregisterWorldObserver(observerTypes[n]);
            }
        }

        /// <summary>
        /// 通过指定的对象类型，注册世界通知的观察对象实例
        /// </summary>
        /// <param name="observerType">观察对象类型</param>
        internal static void RegisterWorldObserver(SystemType observerType)
        {
            Debugger.Assert(observerType.IsClass, "The world observer must be class type.");

            // 静态类特殊处理
            if (NovaEngine.Utility.Reflection.IsTypeOfStaticClass(observerType))
            {
                RegisterWorldObserverOnStaticClass(observerType);
                return;
            }

            // 可实例化的类，需要继承 IWorldObserver 接口
            if (false == typeof(IWorldObserver).IsAssignableFrom(observerType))
            {
                Debugger.Error("The world observer class type '{%t}' must be inherited from 'IWorldObserver', registed it failed.", observerType);
                return;
            }

            // 检查该类型是否已经注册
            for (int n = 0; n < _worldObservers.Count; ++n)
            {
                IWorldObserver worldObserver = _worldObservers[n];
                if (worldObserver.GetType() == observerType)
                {
                    Debugger.Warn("The world observer class type '{%t}' was already register into observer list, repeat added it failed.", observerType);
                    return;
                }
            }

            IWorldObserver observer = System.Activator.CreateInstance(observerType) as IWorldObserver;

            RegisterWorldObserver(observer);
        }

        /// <summary>
        /// 通过指定的对象类型，注销世界通知的观察对象实例
        /// </summary>
        /// <param name="observerType">观察对象类型</param>
        internal static void UnregisterWorldObserver(SystemType observerType)
        {
            Debugger.Assert(observerType.IsClass, "The world observer must be class type.");

            // 静态类特殊处理
            if (NovaEngine.Utility.Reflection.IsTypeOfStaticClass(observerType))
            {
                UnregisterWorldObserverOnStaticClass(observerType);
                return;
            }

            IWorldObserver observer = null;
            // 检查该类型是否已经注册
            for (int n = 0; n < _worldObservers.Count; ++n)
            {
                IWorldObserver worldObserver = _worldObservers[n];
                if (worldObserver.GetType() == observerType)
                {
                    observer = worldObserver;
                    break;
                }
            }

            if (null == observer)
            {
                Debugger.Warn("Could not found any world observer instance with class type '{%t}', removed it failed.", observerType);
                return;
            }

            UnregisterWorldObserver(observer);
        }

        /// <summary>
        /// 通过指定的静态对象类型，注册世界通知的观察对象实例
        /// </summary>
        /// <param name="observerType">观察对象类型</param>
        private static void RegisterWorldObserverOnStaticClass(SystemType observerType)
        {
            if (_worldObserverTypes.ContainsKey(observerType))
            {
                Debugger.Warn("The world observer type '{%t}' was already exist, repeat added it failed.", observerType);
                return;
            }

            IDictionary<string, NovaEngine.Definition.Delegate.EmptyFunctionHandler> dict = new Dictionary<string, NovaEngine.Definition.Delegate.EmptyFunctionHandler>();
            foreach (KeyValuePair<string, NovaEngine.Definition.Delegate.EmptyFunctionHandler> pair in _worldObserverNamesMapping)
            {
                SystemMethodInfo methodInfo = observerType.GetMethod(pair.Key, SystemBindingFlags.Public | SystemBindingFlags.NonPublic | SystemBindingFlags.Static);
                if (null == methodInfo)
                {
                    Debugger.Error("Could not found observer method '{%s}' from static class '{%t}', registed it failed.", pair.Key, observerType);
                    return;
                }

                NovaEngine.Definition.Delegate.EmptyFunctionHandler handler = (NovaEngine.Definition.Delegate.EmptyFunctionHandler) SystemDelegate.CreateDelegate(typeof(NovaEngine.Definition.Delegate.EmptyFunctionHandler), null, methodInfo);
                dict.Add(pair.Key, handler);
            }

            _worldObserverTypes.Add(observerType, dict);

            _startupCallbacks += dict[NovaEngine.Application.ProtocolType.Startup.ToString()];
            _shutdownCallbacks += dict[NovaEngine.Application.ProtocolType.Shutdown.ToString()];
            _fixedExecuteCallbacks += dict[NovaEngine.Application.ProtocolType.FixedExecute.ToString()];
            _executeCallbacks += dict[NovaEngine.Application.ProtocolType.Execute.ToString()];
            _lateExecuteCallbacks += dict[NovaEngine.Application.ProtocolType.LateExecute.ToString()];
            _fixedUpdateCallbacks += dict[NovaEngine.Application.ProtocolType.FixedUpdate.ToString()];
            _updateCallbacks += dict[NovaEngine.Application.ProtocolType.Update.ToString()];
            _lateUpdateCallbacks += dict[NovaEngine.Application.ProtocolType.LateUpdate.ToString()];
        }

        /// <summary>
        /// 通过指定的静态对象类型，注销世界通知的观察对象实例
        /// </summary>
        /// <param name="observerType">观察对象类型</param>
        private static void UnregisterWorldObserverOnStaticClass(SystemType observerType)
        {
            Debugger.Assert(null != observerType, "The world observer type must be non-null.");

            if (null == _worldObserverTypes || false == _worldObserverTypes.TryGetValue(observerType, out IDictionary<string, NovaEngine.Definition.Delegate.EmptyFunctionHandler> dict))
            {
                Debugger.Warn("Could not found any world observer type '{%t}' from cache list, removed it failed.", observerType);
                return;
            }

            _startupCallbacks -= dict[NovaEngine.Application.ProtocolType.Startup.ToString()];
            _shutdownCallbacks -= dict[NovaEngine.Application.ProtocolType.Shutdown.ToString()];
            _fixedExecuteCallbacks -= dict[NovaEngine.Application.ProtocolType.FixedExecute.ToString()];
            _executeCallbacks -= dict[NovaEngine.Application.ProtocolType.Execute.ToString()];
            _lateExecuteCallbacks -= dict[NovaEngine.Application.ProtocolType.LateExecute.ToString()];
            _fixedUpdateCallbacks -= dict[NovaEngine.Application.ProtocolType.FixedUpdate.ToString()];
            _updateCallbacks -= dict[NovaEngine.Application.ProtocolType.Update.ToString()];
            _lateUpdateCallbacks -= dict[NovaEngine.Application.ProtocolType.LateUpdate.ToString()];

            _worldObserverTypes.Remove(observerType);
        }
    }
}
