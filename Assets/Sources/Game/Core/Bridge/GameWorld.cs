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
        /// 当前是否为教程模式
        /// </summary>
        static bool isTutorialMode;

        /// <summary>
        /// 世界容器的启动运行函数
        /// </summary>
        public static void Run()
        {
            // 可能存在开启了教程模式，但是忘记配置具体案例类型的情况
            if (NovaEngine.Configuration.tutorialMode && null != NovaEngine.Configuration.tutorialSampleType)
            {
                // 教程开启
                isTutorialMode = true;
            }
            else
            {
                isTutorialMode = false;
            }
            
            if (isTutorialMode)
            {
                RunGameSample();
            }
            else
            {
                RunGame();
            }
        }

        /// <summary>
        /// 世界容器的停止运行函数
        /// </summary>
        public static void Stop()
        {
            if (isTutorialMode)
            {
                StopGameSample();
            }
            else
            {
                StopGame();
            }
        }

        /// <summary>
        /// 世界容器的重载运行函数
        /// </summary>
        /// <param name="commandType">类型标识</param>
        public static void Reload(GameEngine.EngineCommandType commandType)
        {
            if (isTutorialMode)
            {
                ReloadGameSample(commandType);
            }
            else
            {
                ReloadGame(commandType);
            }
        }

        #region 游戏层访问回调接口

        /// <summary>
        /// 游戏层的启动运行函数
        /// </summary>
        static void RunGame()
        {
            // 加载应用上下文配置
            GameLoader.Load(GameWorldCommandType.Context);

            // 装载热加载模块
            GameEngine.GameApi.AutoRegisterAllHotModulesOfContextConfigure();

            // 加载业务对象类
            GameLoader.Load(GameWorldCommandType.Assembly);

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
        /// 游戏层的停止运行函数
        /// </summary>
        static void StopGame()
        {
            // 关闭应用通知回调接口
            GameEngine.GameLibrary.OnApplicationShutdown(OnApplicationResponseCallback);

            // 清除上下文信息
            CleanupWorldObserverContext();

            // 卸载业务对象类
            GameLoader.Unload(GameWorldCommandType.Assembly);

            // 卸载热加载模块
            GameEngine.GameApi.AutoUnregisterAllHotModulesOfContextConfigure();

            // 卸载应用上下文配置
            GameLoader.Unload(GameWorldCommandType.Context);
        }

        /// <summary>
        /// 游戏层的重载运行函数
        /// </summary>
        /// <param name="commandType">类型标识</param>
        static void ReloadGame(GameEngine.EngineCommandType commandType)
        {
            switch (commandType)
            {
                case GameEngine.EngineCommandType.Hotfix:
                    // 重载业务对象类
                    GameLoader.Reload(GameWorldCommandType.Assembly);
                    break;
                case GameEngine.EngineCommandType.Configure:
                    // 重新导入配置
                    GameLoader.Reload(GameWorldCommandType.Configure);
                    break;
                default:
                    Debugger.Throw<System.InvalidOperationException>($"Invalid reload type {commandType}.");
                    break;
            }
        }

        #endregion

        #region 示例层访问回调接口

        const string TUTORIAL_MODULE_EXTERNAL_GATEWAY_NAME = "GameSample.GameWorld";

        /// <summary>
        /// 示例层的启动运行函数
        /// </summary>
        static void RunGameSample()
        {
            CallRemoteModule(GameEngine.GameMacros.GAME_REMOTE_PROCESS_CALL_RUN_SERVICE_NAME, NovaEngine.Configuration.tutorialSampleType);
        }

        /// <summary>
        /// 示例层的停止运行函数
        /// </summary>
        static void StopGameSample()
        {
            CallRemoteModule(GameEngine.GameMacros.GAME_REMOTE_PROCESS_CALL_STOP_SERVICE_NAME);
        }

        /// <summary>
        /// 示例层的重载运行函数
        /// </summary>
        /// <param name="commandType">类型标识</param>
        static void ReloadGameSample(GameEngine.EngineCommandType commandType)
        {
            CallRemoteModule(GameEngine.GameMacros.GAME_REMOTE_PROCESS_CALL_RELOAD_SERVICE_NAME, commandType);
        }

        /// <summary>
        /// 调用示例层的指定函数
        /// </summary>
        /// <param name="methodName">函数名称</param>
        /// <param name="args">函数参数列表</param>
        private static void CallRemoteModule(string methodName, params object[] args)
        {
            System.Type type = NovaEngine.Utility.Assembly.GetType(TUTORIAL_MODULE_EXTERNAL_GATEWAY_NAME);
            if (type == null)
            {
                Debugger.Error("Could not found tutorial class '{%s}' with current assemblies list, call remote function '{%s}' failed.", TUTORIAL_MODULE_EXTERNAL_GATEWAY_NAME, methodName);
                return;
            }

            // Debugger.Info("Call remote service {%s} with target function name {%s}.", TUTORIAL_MODULE_EXTERNAL_GATEWAY_NAME, methodName);

            NovaEngine.Utility.Reflection.CallMethod(type, methodName, args);
        }

        #endregion

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
