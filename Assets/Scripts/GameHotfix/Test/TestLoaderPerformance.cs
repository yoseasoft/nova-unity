/// <summary>
/// 2024-06-04 Game Framework Code By Hurley
/// </summary>

using System.Reflection;

namespace Game
{
    /// <summary>
    /// 测试加载器性能
    /// </summary>
    public class TestLoaderPerformance : ITestCase
    {
        public void Startup()
        {
            WorldManager.InitWorldObjects();

            WorldManager.soldier.MoveTo(new UnityEngine.Vector3(100, 20, 50));
            WorldManager.monster.MoveTo(new UnityEngine.Vector3(20, 55, 70));

            WorldManager.PrintAllObjects();
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
                    Debugger.Info("_________________________________________________");
                    System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

                    int count = 10000000;
                    stopwatch.Start();
                    for (int n = 0; n < count; ++n)
                    {
                        WorldManager.soldier.OnSoldierInvokeTest();
                    }
                    stopwatch.Stop();
                    Debugger.Warn($"Run soldier normal call for {count} times, using elapsed time was {stopwatch.ElapsedMilliseconds}.");

                    stopwatch.Restart();
                    for (int n = 0; n < count; ++n)
                    {
                        WorldManager.soldier.Call("Test");
                    }
                    stopwatch.Stop();
                    Debugger.Warn($"Run soldier extend call for {count} times, using elapsed time was {stopwatch.ElapsedMilliseconds}.");
                }
            }
        }
    }
}
