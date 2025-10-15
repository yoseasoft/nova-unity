/// <summary>
/// Game Framework
/// 
/// 创建者：Hurley
/// 创建时间：2025-06-18
/// 功能描述：
/// </summary>

using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// Battle场景加载组件逻辑处理类
    /// </summary>
    public static class BattleLoadComponentSystem
    {
        [OnAwake]
        private static void Awake(this BattleLoadComponent self)
        {
            self.uid_generator = 10000;

            self.player = null;
            self.monsters = new List<Monster>();
        }

        [OnStart]
        private static void Start(this BattleLoadComponent self)
        {
        }

        [OnDestroy]
        private static void Destroy(this BattleLoadComponent self)
        {
            if (null != self.player)
            {
                GameEngine.ApplicationContext.ReleaseBean(self.player);
                self.player = null;
            }

            for (int n = 0; n < self.monsters.Count; ++n)
            {
                Monster monster = self.monsters[n];
                GameEngine.ApplicationContext.ReleaseBean(monster);
            }

            self.monsters.Clear();
            self.monsters = null;
        }

        public static void OnSoldierJoin(this BattleLoadComponent self)
        {
            Soldier soldier = GameEngine.ApplicationContext.CreateBean("soldier") as Soldier;

            IdentityComponent identityComponent = soldier.GetComponent<IdentityComponent>();
            identityComponent.objectId = self.GenerateUid();
            identityComponent.objectType = BattleObjectType.Soldier;

            TransformComponent transformComponent = soldier.GetComponent<TransformComponent>();
            transformComponent.position = new Vector3(0f, 0.5f, 0f);
            transformComponent.rotation = new Vector3(0f, 0f, 0f);
            transformComponent.scale = Vector3.one;

            AttributeComponent attributeComponent = soldier.GetComponent<AttributeComponent>();
            attributeComponent.health = 1000;
            attributeComponent.attack = 50;
            attributeComponent.defense = 10;

            BattleMapComponent mapComponent = self.GetComponent<BattleMapComponent>();
            mapComponent.OnActorJoinTheMap(soldier);

            self.player = soldier;

            BattleActorJoinNotify notify = new BattleActorJoinNotify();
            notify.uid = identityComponent.objectId;
            notify.asset_url = "Assets/_Resources/Model/player/h_role02/prefab/h_role02.prefab";
            GameEngine.GameApi.Send(notify);
        }

        public static void OnMonsterJoin(this BattleLoadComponent self, int code)
        {
            string bean_name = code switch
            {
                2 => "sword_monster",
                3 => "bow_monster",
                _ => null
            };

            string asset_url = code switch
            {
                2 => "Assets/_Resources/Model/monster/m_slime01/prefab/m_slime01.prefab",
                3 => "Assets/_Resources/Model/monster/m_gebulin01/prefab/m_gebulin01.prefab",
                _ => null
            };

            Monster monster = GameEngine.ApplicationContext.CreateBean(bean_name) as Monster;

            IdentityComponent identityComponent = monster.GetComponent<IdentityComponent>();
            identityComponent.objectId = self.GenerateUid();
            identityComponent.objectType = BattleObjectType.Monster;

            TransformComponent transformComponent = monster.GetComponent<TransformComponent>();
            transformComponent.position = new Vector3(0f, 0.5f, 0f);
            transformComponent.rotation = new Vector3(0f, 180f, 0f);
            transformComponent.scale = Vector3.one;

            AttributeComponent attributeComponent = monster.GetComponent<AttributeComponent>();
            attributeComponent.health = 100;
            attributeComponent.attack = 20;
            attributeComponent.defense = 5;

            BattleMapComponent mapComponent = self.GetComponent<BattleMapComponent>();
            mapComponent.OnActorJoinTheMap(monster);

            self.monsters.Add(monster);

            BattleActorJoinNotify notify = new BattleActorJoinNotify();
            notify.uid = identityComponent.objectId;
            notify.asset_url = asset_url;
            GameEngine.GameApi.Send(notify);
        }

        public static void OnMonsterLeave(this BattleLoadComponent self, Actor actor)
        {
            Monster monster = actor as Monster;

            IdentityComponent identityComponent = monster.GetComponent<IdentityComponent>();
            int uid = identityComponent.objectId;

            BattleMapComponent mapComponent = self.GetComponent<BattleMapComponent>();
            int index = mapComponent.OnActorLeaveTheMap(monster);

            for (int n = 0; n < self.monsters.Count; ++n)
            {
                if (self.monsters[n].GetComponent<IdentityComponent>().objectId == uid)
                {
                    self.monsters.RemoveAt(n);
                    break;
                }
            }

            GameEngine.ApplicationContext.ReleaseBean(monster);

            BattleActorLeaveNotify notify = new BattleActorLeaveNotify();
            notify.uid = uid;
            notify.index = index;
            GameEngine.GameApi.Send(notify);
        }

        private static int GenerateUid(this BattleLoadComponent self)
        {
            return self.uid_generator++;
        }

        public static Actor FindActorByUid(this BattleLoadComponent self, int uid)
        {
            if (null != self.player)
            {
                IdentityComponent identityComponent = self.player.GetComponent<IdentityComponent>();
                if (identityComponent.objectId == uid)
                {
                    return self.player;
                }
            }

            for (int n = 0; n < self.monsters.Count; ++n)
            {
                IdentityComponent identityComponent = self.monsters[n].GetComponent<IdentityComponent>();
                if (identityComponent.objectId == uid)
                {
                    return self.monsters[n];
                }
            }

            return null;
        }

    }
}
