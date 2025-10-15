/// <summary>
/// 2024-04-09 Game Framework Code By Hurley
/// </summary>

using UnityEngine;

namespace Game
{
    /// <summary>
    /// 移动组件逻辑
    /// </summary>
    public static class MoveComponentSystem
    {
        public static void MoveUp(this MoveComponent self)
        {
            BattleMapComponent mapComponent = NE.SceneHandler.GetCurrentScene().GetComponent<BattleMapComponent>();

            IdentityComponent identityComponent = self.GetComponent<IdentityComponent>();

        }

        [GameEngine.EventSubscribeBindingOfTarget(1201)]
        public static void OnSpawn(this MoveComponent self, int eventID, params object[] args)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            sb.AppendFormat("测试'{0} - {1}'对象监听事件标识'{2}'的带参扩展静态函数'OnSpawn'调用, 参数值:",
                    self.Entity.GetType().FullName, self.GetType().FullName, eventID);
            for (int n = 0; null != args && n < args.Length; ++n)
            {
                sb.AppendFormat("[{0}] = {1}, ", n, args[n].ToString());
            }
            Debugger.Log(sb.ToString());
        }

        [GameEngine.EventSubscribeBindingOfTarget(1202)]
        public static void OnSpawnWithNullParameter(this MoveComponent self)
        {
            Debugger.Log("测试'{0} - {1}'对象监听事件标识'1202'的无参扩展静态函数'OnSpawnWithNullParameter'调用.",
                    self.Entity.GetType().FullName, self.GetType().FullName);
        }

        [GameEngine.EventSubscribeBindingOfTarget(1001)]
        public static void OnDie(this MoveComponent self, int eventID, params object[] args)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            sb.AppendFormat("测试'{0} - {1}'对象监听事件标识'{2}'的带参扩展静态函数'OnDie'调用, 参数值:",
                    self.Entity.GetType().FullName, self.GetType().FullName, eventID);
            for (int n = 0; null != args && n < args.Length; ++n)
            {
                sb.AppendFormat("[{0}] = {1}, ", n, args[n].ToString());
            }
            Debugger.Log(sb.ToString());
        }

        [OnUpdate]
        public static void OnMoveComponentUpdate(this MoveComponent self)
        {
            // Debugger.Warn(" move component update !!!");
        }
    }
}
