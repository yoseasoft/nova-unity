/// <summary>
/// 2025-01-02 Game Framework Code By Hurley
/// </summary>

namespace Game
{
    /// <summary>
    /// 世界管理器的观察者实现类
    /// </summary>
    [WorldObserverAutoBound]
    public static class SimpleGameWorld
    {
        public static void Startup()
        {
            StartGame();
        }

        public static void Shutdown()
        {
            StopGame();
        }

        public static void FixedUpdate()
        { }

        public static void Update()
        { }

        public static void LateUpdate()
        { }

        public static void FixedExecute()
        { }

        public static void Execute()
        { }

        public static void LateExecute()
        { }

        private static void StartGame()
        {
            GameLoader.Load(GameWorldCommandType.Configure);

            // 初始加载logo场景
            NE.SceneHandler.ReplaceScene<LogoScene>();
        }

        private static void StopGame()
        {
        }

        #region 以实例化的形式注册世界通知的观察对象

        /// <summary>
        /// 世界管理器的观察者实现类
        /// </summary>
        /*
        public class SimpleGameWorld : IWorldObserver
        {
            public void Startup() { }

            public void Shutdown() { }

            public void FixedUpdate() { }

            public void Update() { }

            public void LateUpdate() { }

            public void FixedExecute() { }

            public void Execute() { }

            public void LateExecute() { }
        }
        */

        #endregion
    }
}
