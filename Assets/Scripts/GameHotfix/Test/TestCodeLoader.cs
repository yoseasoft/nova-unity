/// <summary>
/// 2024-06-21 Game Framework Code By Hurley
/// </summary>

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Game
{
    /// <summary>
    /// 测试代码加载逻辑
    /// </summary>
    public class TestCodeLoader : ITestCase
    {
        public void Startup()
        {
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
                    Test1();
                }
                else if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.B))
                {
                    Test2();
                }
                else if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.D))
                {
                    Test3();
                }
            }
        }

        private Soldier soldier = null;

        public void Test1()
        {
            if (null == soldier)
            {
                Debugger.Info("创建soldier实例!");
                soldier = (Soldier) GameEngine.InjectController.Instance.CreateBean("soldier");
                // GameEngine.InjectBeanService.AutowiredProcessingOnCreateTargetObject(soldier);
            }

            //AttackComponent attackComponent = soldier.GetComponent<AttackComponent>();
            //attackComponent.OnAttack();
            soldier.OnBuffActivated();
        }

        public void Test2()
        {
            if (null != soldier)
            {
                Debugger.Info("销毁soldier实例!");
                // GameEngine.InjectBeanService.AutowiredProcessingOnReleaseTargetObject(soldier);
                GameEngine.InjectController.Instance.ReleaseBean(soldier);
                soldier = null;
            }
        }

        public void Test3()
        {
            if (null != soldier)
            {
                System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

                int count = 100000;
                stopwatch.Start();
                for (int n = 0; n < count; ++n)
                {
                    // soldier.Call(soldier.Update, GameEngine.CBase.LifecycleKeypointType.Work);
                }
                stopwatch.Stop();
                Debugger.Warn($"Run soldier for call for {count} times, using elapsed time was {stopwatch.ElapsedMilliseconds}.");
            }
        }
    }
}
