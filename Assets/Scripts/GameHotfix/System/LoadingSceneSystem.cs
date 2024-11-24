/// <summary>
/// 2024-05-29 Game Framework Code By Hurley
/// </summary>

using Cysharp.Threading.Tasks;

using GameEngine;

namespace Game
{
    /// <summary>
    /// Loading场景逻辑处理类
    /// </summary>
    [GameEngine.Aspect]
    public static class LoadingSceneSystem
    {
        // public delegate void OnMoveComponentMoveOnHandler<in T>(T self, int x, int y, int z);

        [GameEngine.OnAspectAfterCallOfTarget(typeof(LoadingScene), GameEngine.AspectBehaviourType.Awake)]
        public static void Awake(this LoadingScene self)
        {
            Debugger.Log("欢迎进入Loading场景！");

            self.AddComponent<LoadingPropComponent>();
        }

        [GameEngine.OnAspectAfterCallOfTarget(typeof(LoadingScene), GameEngine.AspectBehaviourType.Start)]
        public static void Start(this LoadingScene self)
        {
            // TestController.SetTargetCaseEnabled("TestProtoLifecycle");
            // TestController.SetTargetCaseEnabled<TestNetworkConn>();
            // TestController.SetTargetCaseEnabled<TestProtoEvent>();
            // TestController.SetTargetCaseEnabled<TestLoaderPerformance>();
            // TestController.SetTargetCaseEnabled<TestAsyncTask>();
            TestController.SetTargetCaseEnabled("TestCodeSnippet");
            // TestController.SetTargetCaseEnabled("TestCodeLoader");
            // TestController.SetTargetCaseEnabled<TestClassType>();
            TestController.Startup();

            NE.SceneHandler.UnloadSceneAsset("20101");
            NE.TimerHandler.Schedule(2000, 1, delegate (int sessionID)
            {
                Debugger.Log("call timer with session {0}.", sessionID);

                //string assetUrl = "Assets/_Resources/scene/level/20101.unity";
                //this.m_loadingScene = NE.SceneHandler.LoadSceneAsset("20101", assetUrl);
            });

            // NE.NetworkHandler.OnMessageDistributeCallDispatched(Proto.ProtoOpcode.PingResp, new Proto.PingResp() { Str = "yukie", SecTime = 10101, MilliTime = 20202 });

            CreateLoadingPanel().Forget();
        }

        [GameEngine.OnAspectAfterCallOfTarget(typeof(LoadingScene), GameEngine.AspectBehaviourType.Destroy)]
        public static void Destroy(this LoadingScene self)
        {
            WorldManager.CleanupWorldObjects();

            TestController.Shutdown();
        }

        [GameEngine.OnAspectAfterCallOfTarget(typeof(LoadingScene), GameEngine.AspectBehaviourType.Update)]
        public static void Update(this LoadingScene self)
        {
            TestController.Update();

            LoadingPropComponent loadingPropComponent = self.GetComponent<LoadingPropComponent>();

            if (null != loadingPropComponent.loadingScene)
            {
                loadingPropComponent.loadingProgress = loadingPropComponent.loadingScene.Progress * 100f; // * 0.3f;
                Debugger.Log("场景加载进度 {0}%", loadingPropComponent.loadingProgress);

                if (loadingPropComponent.loadingScene.IsDone)
                {
                    Debugger.Log("场景加载完成");

                    loadingPropComponent.loadingScene = null;
                    loadingPropComponent.loadingProgress = 0;

                    GameEngine.SceneHandler.Instance.ReplaceScene<BattleScene>();
                }

                return;
            }
        }

        /// <summary>
        /// 创建Loading界面
        /// </summary>
        static async UniTaskVoid CreateLoadingPanel()
        {
            LoadingPanel loadingPanel = (LoadingPanel) await GuiHandler.Instance.OpenUI("LoadingPanel");
            loadingPanel?.StartProgress(66);
        }
    }
}
