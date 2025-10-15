/// <summary>
/// Game Framework
/// 
/// 创建者：Hurley
/// 创建时间：2025-06-19
/// 功能描述：
/// </summary>

using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public enum CmdType: byte
    {
        Unknown,
        MoveUp,
        MoveDown,
        MoveLeft,
        MoveRight,
        Attack,
        Hurt,
        Die
    }
    /// <summary>
    /// 角色指令
    /// </summary>
    public struct BattleActorCmdNotify
    {
        public int uid;
        public CmdType cmd;
    }

    /// <summary>
    /// Battle场景指令组件逻辑处理类
    /// </summary>
    public static class BattleCmdComponentSystem
    {
        [OnAwake]
        private static void Awake(this BattleCmdComponent self)
        {
        }

        [OnStart]
        private static void Start(this BattleCmdComponent self)
        {
        }

        [OnDestroy]
        private static void Destroy(this BattleCmdComponent self)
        {
        }

        [GameEngine.EventSubscribeBindingOfTarget(typeof(BattleActorCmdNotify))]
        private static void OnRecvEventByType(this BattleCmdComponent self, BattleActorCmdNotify eventData)
        {
            BattleMapComponent map = self.GetComponent<BattleMapComponent>();
            int index = map.FindGridByUid(eventData.uid);
            if (index < 0)
            {
                Debugger.Error("没有找到该角色的位置？");
                return;
            }

            BattleLoadComponent load = self.GetComponent<BattleLoadComponent>();
            Actor actor = load.FindActorByUid(eventData.uid);

            switch (eventData.cmd)
            {
                case CmdType.MoveUp:
                case CmdType.MoveDown:
                case CmdType.MoveLeft:
                case CmdType.MoveRight:
                    map.OnActorMove(actor, eventData.cmd);
                    break;

                case CmdType.Attack:
                    actor.GetComponent<AttackComponent>()?.OnAttack();
                    break;
            }
        }

    }
}
