/// <summary>
/// 2024-04-26 Game Framework Code By Hurley
/// </summary>

namespace Game
{
    /// <summary>
    /// 测试原型对象生命周期
    /// </summary>
    public class TestProtoLifecycle : ITestCase
    {
        public void Startup()
        {
            WorldManager.InitWorldObjects();

            WorldManager.soldier.AddComponent<BodyComponent>();
            WorldManager.soldier.AddComponent<EffectComponent>();

            Debugger.Warn("==================================================> end TestProtoLifecycle <==================================================");
        }

        public void Shutdown()
        {
        }

        public void Update()
        {
            if (UnityEngine.Input.anyKey)
            {
                if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.A))
                {
                    GameReg.ReloadClass();
                }
            }
        }
    }
}
