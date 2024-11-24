/// <summary>
/// 2023-08-25 Game Framework Code By Hurley
/// </summary>

using AssetModule;

namespace Game
{
    /// <summary>
    /// 程序的世界管理容器入口封装对象类，提供底层调度业务层的总入口
    /// </summary>
    public static partial class GameWorld
    {
        public static async void Startup()
        {
            // 初始化资源模块
            await AssetManagement.InitAsync().Task;

            // 业务数据加载
            await GameLoader.Load();

            // 初始加载logo场景
            GameEngine.SceneHandler.Instance.ReplaceScene<LogoScene>();
        }

        public static void Shutdown()
        {
        }

        public static void FixedUpdate()
        {
        }

        public static void Update()
        {
        }

        public static void LateUpdate()
        {
        }
    }
}