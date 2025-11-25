/// <summary>
/// Game Framework
/// 
/// 创建者：Hurley
/// 创建时间：2025-06-16
/// 功能描述：
/// </summary>

namespace Game
{
    /// <summary>
    /// Logo场景转场逻辑处理类
    /// </summary>
    public static class LogoSceneJumpingSystem
    {
        [GameEngine.OnAspectAfterCall(GameEngine.AspectBehaviourType.Awake)]
        private static void Awake(this LogoScene self)
        {
        }

        [GameEngine.OnAspectAfterCall(GameEngine.AspectBehaviourType.Start)]
        private static void Start(this LogoScene self)
        {
            Debugger.Log("欢迎进入Logo场景的跳转相关业务流程！");
        }

        [GameEngine.OnAspectAfterCall(GameEngine.AspectBehaviourType.Destroy)]
        private static void Destroy(this LogoScene self)
        {
            Debugger.Log("当前正在销毁并退出Logo场景的跳转相关业务流程！");
        }

        [GameEngine.OnInputDispatchCall((int) UnityEngine.KeyCode.A, GameEngine.InputOperationType.Released)]
        private static void OnLogoSceneClicked(int keycode, int operationType)
        {
            LogoScene scene = NE.SceneHandler.GetCurrentScene() as LogoScene;
            if (null == scene) return;

            Debugger.Log($"在Logo场景以{operationType}方式按下{keycode}键！");

            LogoTimerComponent tc = scene.GetComponent<LogoTimerComponent>();
            tc.StopTimer();

            LogoTransitionCountdownComponent tcc = scene.GetComponent<LogoTransitionCountdownComponent>();
            tcc.StartTransitionCountdown();

            //NE.SceneHandler.ReplaceScene<WorldScene>();
        }

        [GameEngine.OnEventDispatchCall(typeof(LogoScene), 1201)]
        private static void OnLogoSceneRecvTransitionCountdownNotify(LogoScene scene, int eventID, params object[] args)
        {
            Debugger.Log("Logo场景对象接收到事件标识‘{%d}’的通知，准备处理转场逻辑！", eventID);

            scene.Call("LogoTest");
            NE.SceneHandler.ReplaceScene<LoadingScene>();

            // Debugger.Log("即将进入主业务场景！");
            // GameEngine.SceneHandler.Instance.ReplaceScene((int) ESceneType.Loading);
        }
    }
}
