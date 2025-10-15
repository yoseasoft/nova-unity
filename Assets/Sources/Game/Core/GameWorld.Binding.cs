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

            // 初始化上下文信息
            InitWorldObserverContext();

            System.Collections.Generic.IList<GameEngine.Loader.Symboling.SymClass> list = GameEngine.Loader.CodeLoader.FindAllSymClassesByFeatureType(typeof(WorldObserverAutoBoundAttribute));
            for (int n = 0; n < list.Count; ++n)
            {
                RegisterWorldObserver(list[n].ClassType);
            }

            // 启动应用通知回调接口
            GameEngine.GameLibrary.OnApplicationStartup(OnApplicationResponseCallback);
        }

        /// <summary>
        /// 世界容器的停止运行函数
        /// </summary>
        public static void Stop()
        {
            // 关闭应用通知回调接口
            GameEngine.GameLibrary.OnApplicationShutdown(OnApplicationResponseCallback);

            // 清除上下文信息
            CleanupWorldObserverContext();

            // 卸载业务对象类
            GameReg.UnloadClass();
        }

        /// <summary>
        /// 世界容器的重载运行函数
        /// </summary>
        /// <param name="type">类型标识</param>
        public static void Reload(int type)
        {
            switch (type)
            {
                case (int) GameEngine.EngineCommandType.Hotfix:
                    // 重载业务对象类
                    GameReg.ReloadClass();
                    break;
                case (int) GameEngine.EngineCommandType.Configure:
                    // 重新导入配置
                    ReloadConfigure();
                    break;
                default:
                    Debugger.Throw<System.InvalidOperationException>($"Invalid reload type {type}.");
                    break;
            }
        }

        /// <summary>
        /// 世界容器的重新导入配置函数
        /// </summary>
        static async void ReloadConfigure()
        {
            // 重新导入配置数据
            await ConfigLoader.ReloadAsync();
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
                case NovaEngine.Application.ProtocolType.FixedExecute:
                    FixedExecute();
                    break;
                case NovaEngine.Application.ProtocolType.Execute:
                    Execute();
                    break;
                case NovaEngine.Application.ProtocolType.LateExecute:
                    LateExecute();
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
