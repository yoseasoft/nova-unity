/// <summary>
/// 2023-08-28 Game Framework Code By Hurley
/// </summary>

namespace Game
{
    /// <summary>
    /// 程序的世界管理容器入口封装对象类，提供底层调度业务层的总入口
    /// </summary>
    public static partial class GameWorld
    {
        /// <summary>
        /// 世界容器的启动运行函数
        /// </summary>
        public static void Run()
        {
            // 加载业务对象类
            GameReg.LoadClass();

            // 添加应用通知回调接口
            GameEngine.EngineDispatcher.AddApplicationResponseHandler(OnApplicationResponseCallback);

            GameEngine.EngineDispatcher.OnDispatchingStartup();
        }

        /// <summary>
        /// 世界容器的停止运行函数
        /// </summary>
        public static void Stop()
        {
            GameEngine.EngineDispatcher.OnDispatchingShutdown();

            // 移除应用通知回调接口
            GameEngine.EngineDispatcher.RemoveApplicationResponseHandler(OnApplicationResponseCallback);

            // 卸载业务对象类
            GameReg.UnloadClass();
        }

        /// <summary>
        /// 世界容器的重启运行函数
        /// </summary>
        public static void Reload()
        {
            // 重载业务对象类
            GameReg.ReloadClass();
        }

        /// <summary>
        /// 应用层相应通知回调函数
        /// </summary>
        /// <param name="protocolType">通知协议类型</param>
        private static void OnApplicationResponseCallback(NovaEngine.Application.ProtocolType protocolType)
        {
            switch (protocolType)
            {
                case NovaEngine.Application.ProtocolType.Startup:
                    Startup();
                    break;
                case NovaEngine.Application.ProtocolType.Shutdown:
                    Shutdown();
                    break;
                case NovaEngine.Application.ProtocolType.FixedUpdate:
                    FixedUpdate();
                    break;
                case NovaEngine.Application.ProtocolType.Update:
                    Update();
                    break;
                case NovaEngine.Application.ProtocolType.LateUpdate:
                    LateUpdate();
                    break;
                default:
                    break;
            }
        }
    }
}
