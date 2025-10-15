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
            Debugger.Warn("当前正在运行 Bullet ({%p}) 对象的构造函数……", this);
        }

        ~Bullet()
        {
            Debugger.Warn("当前正在运行 Bullet ({%p}) 对象的析构函数……", this);
        }

        public void Fire()
        {
            Debugger.Log("子弹向目标对象射出!");
        }
    }
}
