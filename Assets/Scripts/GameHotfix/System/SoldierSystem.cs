/// <summary>
/// 2024-04-09 Game Framework Code By Hurley
/// </summary>

using UnityEngine;

namespace Game
{
    /// <summary>
    /// 对象逻辑
    /// </summary>
    [GameEngine.Aspect, GameEngine.ExtendSupported]
    public static class SoldierSystem
    {
        public static void MoveOn(this MoveComponent self, float x, float y, float z)
        {
            Vector3 pos = new Vector3(x, y, z);
            self.MoveTo(pos);
        }

        [GameEngine.OnAspectBeforeCallOfTarget(typeof(Soldier), GameEngine.AspectBehaviourType.Start)]
        public static void BeforeStart(this Soldier self)
        {
            Debugger.Log("测试'{0}'对象在'Start'前的回调处理流程,此时对象的生命周期状态为'{1}'!", self.GetType().FullName, self.CurrentLifecycleRunningStep);
            // NE.ObjectHandler.DestroyObject(self);
        }

        [GameEngine.EventSubscribeBindingOfTarget(1001, GameEngine.AspectBehaviourType.Awake)]
        public static void ShowName(this Soldier self, int eventID, params object[] args)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            sb.AppendFormat("测试'{0}'对象针对事件标识'{1}'的带参扩展静态函数'ShowName'调用, 参数值:", self.GetType().FullName, eventID);
            for (int n = 0; null != args && n < args.Length; ++n)
            {
                sb.AppendFormat("[{0}] = {1}, ", n, args[n].ToString());
            }
            Debugger.Log(sb.ToString());
        }

        [GameEngine.EventSubscribeBindingOfTarget(1001, GameEngine.AspectBehaviourType.Start)]
        public static void ShowNameWithNullParameter(this Soldier self)
        {
            Debugger.Log("测试'{0}'对象针对事件标识'1001'的无参扩展静态函数'ShowNameWithNullParameter'调用.", self.GetType().FullName);
        }

        [GameEngine.EventSubscribeBindingOfTarget(1202, GameEngine.AspectBehaviourType.Awake)]
        public static void ShowBiuBiuBiuWithNullParameter(this Soldier self)
        {
            Debugger.Log("测试'{0}'对象针对事件标识'1202'的无参扩展静态函数'ShowBiuBiuBiuWithNullParameter'调用.", self.GetType().FullName);
        }

        [GameEngine.OnAspectAfterCallOfTarget(typeof(Soldier), GameEngine.AspectBehaviourType.Update)]
        public static void OnSoldierUpdate(this Soldier self)
        {
            // Debugger.Info("测试'{0}'对象的刷新调度操作!", self.GetType().FullName);
        }

        [GameEngine.OnAspectExtendCallOfTarget(typeof(Soldier), "Test")]
        public static void OnSoldierExtendTest(this Soldier self)
        {
            // Debugger.Log("Exec soldier extend test ...");
        }

        public static void OnSoldierInvokeTest(this Soldier self)
        {
            // Debugger.Log("Exec soldier invoke test ...");
        }
    }
}
