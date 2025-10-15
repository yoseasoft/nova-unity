using GameEngine;

namespace Game
{
    /// <summary>
    /// Buff对象类
    /// </summary>
    [GameEngine.Inject]
    public class Buff : CObject
    {
        public Buff()
        {
            Debugger.Warn("当前正在运行 Buff ({%p}) 对象的构造函数……", this);
        }

        ~Buff()
        {
            Debugger.Warn("当前正在运行 Buff ({%p}) 对象的析构函数……", this);
        }

        public void OnActivated()
        {
            Debugger.Log("成功激活当前的 Buff ({%p}) 对象效果!", this);
        }
    }
}
