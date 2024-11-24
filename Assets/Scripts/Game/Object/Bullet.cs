using GameEngine;

namespace Game
{
    /// <summary>
    /// 子弹对象类
    /// </summary>
    public class Bullet : CObject
    {
        public Bullet()
        {
            Debugger.Warn("Bullet Construct Method Running ...");
        }

        ~Bullet()
        {
            Debugger.Warn("Bullet Destruct Method Running ...");
        }

        public void Fire()
        {
            Debugger.Log("子弹向目标对象射出!");
        }
    }
}
