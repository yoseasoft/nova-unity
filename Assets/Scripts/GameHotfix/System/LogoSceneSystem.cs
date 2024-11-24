/// <summary>
/// 2024-05-29 Game Framework Code By Hurley
/// </summary>

namespace Game
{
    /// <summary>
    /// LOGO场景逻辑处理类
    /// </summary>
    [GameEngine.Aspect]
    public static class LogoSceneSystem
    {
        [GameEngine.OnAspectAfterCallOfTarget(typeof(LogoScene), GameEngine.AspectBehaviourType.Awake)]
        public static void Awake(this LogoScene self)
        {
            self.AddComponent<LogoPropComponent>();
        }

        [GameEngine.OnAspectAfterCallOfTarget(typeof(LogoScene), GameEngine.AspectBehaviourType.Start)]
        public static void Start(this LogoScene self)
        {
            Debugger.Log("欢迎进入Logo场景！");

            LogoPropComponent logoPropComponent = self.GetComponent<LogoPropComponent>();
            logoPropComponent.enterSceneTimestamp = NovaEngine.Facade.Timestamp.RealtimeSinceStartup;
            logoPropComponent.countdownTime = 4;

            self.Schedule(2000, -1, delegate (int s)
            {
                Debugger.Log("logo schedule first on {0}.", s);
            });

            self.Schedule(1000, -1, delegate (int s)
            {
                Debugger.Log("logo schedule second on {0}.", s);
            });

            self.Schedule(3000, -1, delegate (int s)
            {
                Debugger.Log("logo schedule third on {0}.", s);
            });

            TestLogoEventDataInfo di = new TestLogoEventDataInfo() { logoID = 100, logoName = "yukie", logoType = 201, };
            GameEngine.EventController.Instance.Send(1001, "yukie", "fire", 205);
            GameEngine.EventController.Instance.Send(di);
            GameEngine.EventController.Instance.Fire(new TestLogoEventDataInfo { logoID = 200, logoName = "shiyue", logoType = 505 });

            // AccountManager.Connect((int) NovaEngine.NetworkServiceType.Tcp, "main", "10.1.90.94:24062");
        }

        [GameEngine.OnAspectAfterCallOfTarget(typeof(LogoScene), GameEngine.AspectBehaviourType.Destroy)]
        public static void Destroy(this LogoScene self)
        {
            Debugger.Log("当前正在销毁并退出Logo场景！");
        }

        [GameEngine.OnAspectAfterCallOfTarget(typeof(LogoScene), GameEngine.AspectBehaviourType.Update)]
        public static void Update(this LogoScene self)
        {
            LogoPropComponent logoPropComponent = self.GetComponent<LogoPropComponent>();

            float now = NovaEngine.Facade.Timestamp.RealtimeSinceStartup;
            if (now - logoPropComponent.enterSceneTimestamp > 1.0f)
            {
                logoPropComponent.enterSceneTimestamp = now;
                --logoPropComponent.countdownTime;

                if (logoPropComponent.countdownTime > 0)
                {
                    Debugger.Log("即将进入主业务场景，还剩 {0} 秒！", logoPropComponent.countdownTime);
                }
                else
                {
                    self.Call("Test");
                    GameEngine.SceneHandler.Instance.ReplaceScene<LoadingScene>();
                }
            }
            // Debugger.Log("即将进入主业务场景！");
            // GameEngine.SceneHandler.Instance.ReplaceScene((int) ESceneType.Loading);
        }
    }
}
