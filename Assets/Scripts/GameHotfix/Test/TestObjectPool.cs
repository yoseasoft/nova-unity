/// <summary>
/// 2024-04-29 Game Framework Code By Hurley
/// </summary>

namespace Game
{
    /// <summary>
    /// 测试对象池操作
    /// </summary>
    public class TestObjectPool : ITestCase
    {
        public void Startup()
        {
            System.Type soldierType = typeof(Soldier);

            /*
            Soldier s1 = GameEngine.PoolController.Instance.CreateObject(soldierType) as Soldier;
            Soldier s2 = GameEngine.PoolController.Instance.CreateObject(soldierType) as Soldier;

            Debugger.Warn("111 s1 = {0}, s2 = {1}", s1.ToString(), s2.ToString());

            GameEngine.PoolController.Instance.ReleaseObject(s1);
            GameEngine.PoolController.Instance.ReleaseObject(s2);

            Debugger.Warn("---------------------------------------");

            s1 = GameEngine.PoolController.Instance.CreateObject(soldierType) as Soldier;
            s2 = GameEngine.PoolController.Instance.CreateObject(soldierType) as Soldier;

            Debugger.Warn("222 s1 = {0}, s2 = {1}", s1.ToString(), s2.ToString());

            GameEngine.PoolController.Instance.ReleaseObject(s1);
            GameEngine.PoolController.Instance.ReleaseObject(s2);

            Debugger.Warn("---------------------------------------");

            NovaEngine.ReferencePool.ClearAllCollections();
            */

            Soldier s1 = NE.ObjectHandler.CreateObject<Soldier>();
            Soldier s2 = NE.ObjectHandler.CreateObject<Soldier>();

            Debugger.Warn("---------------------------------------");

            NE.ObjectHandler.DestroyObject(s1);
            NE.ObjectHandler.DestroyObject(s2);

            Debugger.Warn("---------------------------------------");

            s1 = NE.ObjectHandler.CreateObject<Soldier>();
            s2 = NE.ObjectHandler.CreateObject<Soldier>();

            Debugger.Warn("---------------------------------------");

            Debugger.Warn("==================================================> end TestObjectPool <==================================================");
        }

        public void Shutdown()
        {
        }

        public void Update()
        {
        }
    }
}
