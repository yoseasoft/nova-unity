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
    /// Logo场景转场倒计时组件
    /// </summary>
    public class LogoTransitionCountdownComponent : GameEngine.CComponent
    {
        public float enterSceneTimestamp;

        public int countdownTime;

        public bool running;
        public bool completed;
    }
}
