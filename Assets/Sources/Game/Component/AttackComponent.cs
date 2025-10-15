/// <summary>
/// Game Framework
/// 
/// 创建者：Hurley
/// 创建时间：2025-06-19
/// 功能描述：
/// </summary>

namespace Game
{
    /// <summary>
    /// 攻击组件对象类
    /// </summary>
    [GameEngine.Inject]
    public class AttackComponent : GameEngine.CComponent
    {
        public Bullet bullet;

        public float interval;
        public float lastTime;
    }
}
