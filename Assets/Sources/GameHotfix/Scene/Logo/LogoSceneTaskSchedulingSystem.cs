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
    /// Logo场景任务调度逻辑处理类
    /// </summary>
    public static class LogoSceneTaskSchedulingSystem
    {
        [GameEngine.OnAspectAfterCall(GameEngine.AspectBehaviourType.Awake)]
        public static void Awake(this LogoScene self)
        {
            self.AddComponent<LogoTimerComponent>();
            self.AddComponent<LogoTransitionCountdownComponent>();
        }

        [GameEngine.OnAspectAfterCall(GameEngine.AspectBehaviourType.Start)]
        public static void Start(this LogoScene self)
        {
            Debugger.Log("欢迎进入Logo场景任务调度业务！");
        }

        [GameEngine.OnAspectAfterCall(GameEngine.AspectBehaviourType.Destroy)]
        public static void Destroy(this LogoScene self)
        {
            Debugger.Log("当前正在销毁并退出Logo场景任务调度业务！");
        }
    }
}
