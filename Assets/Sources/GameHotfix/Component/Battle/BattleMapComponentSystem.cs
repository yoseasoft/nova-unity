/// <summary>
/// Game Framework
/// 
/// 创建者：Hurley
/// 创建时间：2025-06-18
/// 功能描述：
/// </summary>

using System.Collections.Generic;

namespace Game
{
    /// <summary>
    /// Battle场景地图组件逻辑处理类
    /// </summary>
    public static class BattleMapComponentSystem
    {
        [OnAwake]
        private static void Awake(this BattleMapComponent self)
        {
            /**
             * 默认地图为 9*9 大小；
             * 每个格子长宽均为 1；
             * 布局为从左往右站位；
             */

            self.width_size = 1f;
            self.height_size = 1f;
            self.width_count = 9;
            self.height_count = 9;

            self.grid_count = self.height_count * self.width_count;
            self.grids = new int[self.grid_count];
        }

        [OnStart]
        private static void Start(this BattleMapComponent self)
        {
        }

        [OnDestroy]
        private static void Destroy(this BattleMapComponent self)
        {
            self.grids = null;
        }

        public static UnityEngine.Vector3 CalcGridPositionByIndex(this BattleMapComponent self, int index)
        {
            if (index >= self.grid_count)
            {
                Debugger.Warn("访问索引越界！");
                return UnityEngine.Vector3.zero;
            }

            int x = index % self.width_count;
            int y = index / self.width_count;
            UnityEngine.Vector3 v = new UnityEngine.Vector3(x * self.width_size + self.width_size / 2f, 0f, y * self.height_size + self.height_size / 2f);
            return v;
        }

        public static int FindGridByUid(this BattleMapComponent self, int uid)
        {
            for (int n = 0; n < self.grids.Length; ++n)
            {
                if (self.grids[n] == uid)
                {
                    return n;
                }
            }

            return -1;
        }

        public static int OnActorJoinTheMap(this BattleMapComponent self, Actor actor)
        {
            IdentityComponent identityComponent = actor.GetComponent<IdentityComponent>();

            int index = self.FindGridByUid(identityComponent.objectId);
            if (index >= 0)
            {
                Debugger.Error($"该角色已经在地图上了，不能重复加入！");
                return index;
            }

            if (identityComponent.objectType == BattleObjectType.Soldier)
            {
                index = (self.height_count / 2) * self.width_count;
                while (self.grids[index] > 0)
                {
                    Debugger.Warn("这里不应该有人啊？");
                    index++;
                }

                self.grids[index] = identityComponent.objectId;
                return index;
            }
            else if (identityComponent.objectType == BattleObjectType.Monster)
            {
                for (int c = self.width_count - 1; c >= 0; --c)
                {
                    for (int r = self.height_count - 1; r >= 0; --r)
                    {
                        index = r * self.width_count + c;
                        if (self.grids[index] <= 0)
                        {
                            self.grids[index] = identityComponent.objectId;
                            return index;
                        }
                    }
                }
            }

            return -1;
        }

        public static int OnActorLeaveTheMap(this BattleMapComponent self, Actor actor)
        {
            IdentityComponent identityComponent = actor.GetComponent<IdentityComponent>();

            int index = self.FindGridByUid(identityComponent.objectId);
            if (index >= 0)
            {
                self.grids[index] = 0;
            }

            return index;
        }

        public static void OnActorMove(this BattleMapComponent self, Actor actor, CmdType type)
        {
            IdentityComponent identityComponent = actor.GetComponent<IdentityComponent>();

            int index = self.FindGridByUid(identityComponent.objectId);
            if (index < 0)
            {
                Debugger.Error($"该角色不在地图上了，移动失败！");
                return;
            }

            int target_index = 0;
            if (CmdType.MoveUp == type)
            {
                target_index = index + self.width_count;
                if (target_index >= self.grid_count || self.grids[target_index] > 0) { Debugger.Warn("目标位置不能移动！"); return; }
            }
            else if (CmdType.MoveDown == type)
            {
                target_index = index - self.width_count;
                if (target_index < 0 || self.grids[target_index] > 0) { Debugger.Warn("目标位置不能移动！"); return; }
            }
            else if (CmdType.MoveLeft == type)
            {
                target_index = index - 1;
                if (index % self.width_count == 0 || self.grids[target_index] > 0) { Debugger.Warn("目标位置不能移动！"); return; }
            }
            else if (CmdType.MoveRight == type)
            {
                target_index = index + 1;
                if (target_index % self.width_count == 0 || self.grids[target_index] > 0) { Debugger.Warn("目标位置不能移动！"); return; }
            }
            else
            {
                Debugger.Warn($"移动的指令类型错误 {type}");
                return;
            }

            self.grids[target_index] = self.grids[index];
            self.grids[index] = 0;

            GameEngine.GameApi.Send(new BattleActorMoveToNotify() { uid = identityComponent.objectId });
        }

        public static IList<int> OnActorAttackByRange(this BattleMapComponent self, Actor actor, int range)
        {
            IList<int> result = new List<int>();

            IdentityComponent identityComponent = actor.GetComponent<IdentityComponent>();
            int index = self.FindGridByUid(identityComponent.objectId);
            if (index < 0)
            {
                Debugger.Error($"该角色不在地图上了，移动失败！");
                return result;
            }

            for (int n = 1; n <= range; ++n)
            {
                int target_index = index + n;
                if (target_index % self.width_count == 0)
                    break;

                int target_uid = self.grids[target_index];
                if (target_uid > 0)
                {
                    result.Add(target_uid);
                }
            }

            return result;
        }

    }
}
