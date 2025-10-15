/// <summary>
/// 2023-08-25 Game Framework Code By Hurley
/// </summary>

using System.Collections.Generic;

namespace Game
{
    /// <summary>
    /// 程序的世界管理容器入口封装对象类，提供底层调度业务层的总入口
    /// </summary>
    public static partial class GameWorld
    {
        [WorldObserverFunctionReference(NovaEngine.Application.ProtocolType.Startup)]
        private static NovaEngine.Definition.Delegate.EmptyFunctionHandler _startupCallbacks;
        [WorldObserverFunctionReference(NovaEngine.Application.ProtocolType.Shutdown)]
        private static NovaEngine.Definition.Delegate.EmptyFunctionHandler _shutdownCallbacks;
        [WorldObserverFunctionReference(NovaEngine.Application.ProtocolType.FixedExecute)]
        private static NovaEngine.Definition.Delegate.EmptyFunctionHandler _fixedExecuteCallbacks;
        [WorldObserverFunctionReference(NovaEngine.Application.ProtocolType.Execute)]
        private static NovaEngine.Definition.Delegate.EmptyFunctionHandler _executeCallbacks;
        [WorldObserverFunctionReference(NovaEngine.Application.ProtocolType.LateExecute)]
        private static NovaEngine.Definition.Delegate.EmptyFunctionHandler _lateExecuteCallbacks;
        [WorldObserverFunctionReference(NovaEngine.Application.ProtocolType.FixedUpdate)]
        private static NovaEngine.Definition.Delegate.EmptyFunctionHandler _fixedUpdateCallbacks;
        [WorldObserverFunctionReference(NovaEngine.Application.ProtocolType.Update)]
        private static NovaEngine.Definition.Delegate.EmptyFunctionHandler _updateCallbacks;
        [WorldObserverFunctionReference(NovaEngine.Application.ProtocolType.LateUpdate)]
        private static NovaEngine.Definition.Delegate.EmptyFunctionHandler _lateUpdateCallbacks;

        public static void Startup()
        {
            _startupCallbacks?.Invoke();
        }

        public static void Shutdown()
        {
            _shutdownCallbacks?.Invoke();
        }

        public static void FixedExecute()
        {
            _fixedExecuteCallbacks?.Invoke();
        }

        public static void Execute()
        {
            _executeCallbacks?.Invoke();
        }

        public static void LateExecute()
        {
            _lateExecuteCallbacks?.Invoke();
        }

        public static void FixedUpdate()
        {
            _fixedUpdateCallbacks?.Invoke();
        }

        public static void Update()
        {
            _updateCallbacks?.Invoke();
        }

        public static void LateUpdate()
        {
            _lateUpdateCallbacks?.Invoke();
        }
    }
}
