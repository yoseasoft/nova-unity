/// <summary>
/// 2024-04-26 Game Framework Code By Hurley
/// </summary>

using System.Reflection;

namespace Game
{
    /// <summary>
    /// 测试原型对象事件功能
    /// </summary>
    public class TestProtoEvent : ITestCase
    {
        public void Startup()
        {
            WorldManager.InitWorldObjects();

            WorldManager.soldier.Send(1001, "yukie", "nice", 502);
            WorldManager.soldier.Fire(new SoldierHurtDataInfo() { damageType = 101, damage = 55, attackerName = "monster" });

            WorldManager.soldier.MoveTo(new UnityEngine.Vector3(100, 20, 50));
            WorldManager.monster.MoveTo(new UnityEngine.Vector3(20, 55, 70));

            WorldManager.PrintAllObjects();

            WorldManager.soldier.Hit(WorldManager.monster);
            WorldManager.soldier.Hit(WorldManager.monster);

            WorldManager.PrintAllObjects();

            // WorldManager.RemoveMonster();

            WorldManager.soldier.Fire(1201, "hello", 10101, "good");
            WorldManager.soldier.Fire(1202, "good", 10201, "night");
            // WorldManager.soldier.SendToSelf(1201, "hello", 10101, "good");

            {
                /*
                System.Type classType = typeof(MoveComponentSystem);
                MethodInfo methodInfo = classType.GetMethod("MoveOn", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
                bool isExtension = NovaEngine.Utility.Reflection.IsTypeOfExtension(methodInfo); // methodInfo.IsDefined(typeof(System.Runtime.CompilerServices.ExtensionAttribute), false);
                Debugger.Log("{0} -------- {1} ------- {2}", methodInfo.Name, methodInfo.DeclaringType.FullName, isExtension);
                ParameterInfo[] parameters = methodInfo.GetParameters();
                for (int n = 0; n < parameters.Length; ++n)
                {
                    Debugger.Log("{0}'s parameter[{1}] = {2}.", methodInfo.Name, n, parameters[n].ParameterType);
                }
                //OnMoveComponentMoveOnHandler<MoveComponent> handler = methodInfo.CreateDelegate(typeof(OnMoveComponentMoveOnHandler<MoveComponent>)) as OnMoveComponentMoveOnHandler<MoveComponent>;
                //handler(WorldManager.soldier.GetComponent<MoveComponent>(), 10, 15, 75);
                System.Delegate callback = NovaEngine.Utility.Reflection.CreateGenericActionDelegate(methodInfo, new System.Type[] {
                    typeof(MoveComponent), typeof(float), typeof(float), typeof(float),
                });
                callback.DynamicInvoke(new object[] { WorldManager.soldier.GetComponent<MoveComponent>(), 10, 15, 75 });
                */
            }

            {
                System.Type classType = typeof(SoldierApiService);
                MethodInfo methodInfo = classType.GetMethod("DoSoldierAttack", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

                //System.Type genericFunc = typeof(System.Func<,,>);
                //System.Type funcType = genericFunc.MakeGenericType(new [] { typeof(Soldier), typeof(int), typeof(int) });
                //System.Delegate actualMethod = System.Delegate.CreateDelegate(funcType, null, methodInfo);
                //object result = actualMethod.DynamicInvoke(new object [] { WorldManager.soldier, 5 });
                //Debugger.Warn("动态函数测试的结果为: {0}", result.ToString());
                System.Delegate callback = NovaEngine.Utility.Reflection.CreateGenericFuncDelegate(methodInfo);
                object result = callback.DynamicInvoke(WorldManager.soldier, 5);
                Debugger.Warn("动态函数测试的结果为: {0}", result.ToString());

                methodInfo = classType.GetMethod("DoSoldierEmpty", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                System.Type genericAction = typeof(System.Action);
                // System.Type actionType = genericAction.MakeGenericType(new System.Type[] { });
                // actualMethod = System.Delegate.CreateDelegate(genericAction, null, methodInfo);
                System.Delegate actualMethod = NovaEngine.Utility.Reflection.CreateGenericActionDelegate(methodInfo);
                actualMethod.DynamicInvoke();
            }

            WorldManager.PrintAllObjects();

            // WorldManager.CleanupWorldObjects();

            Debugger.Warn("==================================================> end TestProtoEvent <==================================================");
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
                    WorldManager.soldier.Fire(1202, "good", 10201, "night");
                }
            }
        }
    }
}
