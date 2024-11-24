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
            Debugger.Warn("Buff Construct Method Running ...");
        }

        ~Buff()
        {
            Debugger.Warn("Buff Destruct Method Running ...");
        }

        public void OnActivated()
        {
            Debugger.Log("Buff激活效果!");
        }
    }
}
