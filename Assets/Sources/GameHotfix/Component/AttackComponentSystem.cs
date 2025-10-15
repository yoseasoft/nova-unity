/// <summary>
/// Game Framework
/// 
/// 创建者：Hurley
/// 创建时间：2025-06-19
/// 功能描述：
/// </summary>

using System.Collections.Generic;

namespace Game
{
    /// <summary>
    /// 攻击组件对象逻辑处理类
    /// </summary>
    public static class AttackComponentSystem
    {
        [OnAwake]
        private static void Awake(this AttackComponent self)
        {
            self.interval = 2f;
            self.lastTime = NovaEngine.Timestamp.RealtimeSinceStartup;
        }

        [OnStart]
        private static void Start(this AttackComponent self)
        {
        }

        [OnDestroy]
        private static void Destroy(this AttackComponent self)
        {
        }

        public static void OnAttack(this AttackComponent self)
        {
            float now = NovaEngine.Timestamp.RealtimeSinceStartup;
            if (self.lastTime + self.interval > now)
            {
                Debugger.Log("攻击需要一定的间隔时间，请歇息一会再继续攻击！");
                return;
            }

            BattleScene scene = NE.SceneHandler.GetCurrentScene() as BattleScene;
            if (null == scene) return;

            BattleMapComponent mapComponent = scene.GetComponent<BattleMapComponent>();
            BattleLoadComponent loadComponent = scene.GetComponent<BattleLoadComponent>();

            IList<int> targets = null;
            string skill = "";
            if (null == self.bullet)
            {
                Debugger.Log($"{self.Entity.GetType().FullName}用普通攻击的方式击打目标!");
                targets = mapComponent.OnActorAttackByRange(self.Entity as Actor, 1);
                skill = "skill01";
            }
            else
            {
                Debugger.Log($"{self.Entity.GetType().FullName}用子弹攻击的方式击打目标！");
                self.bullet.Fire();

                targets = mapComponent.OnActorAttackByRange(self.Entity as Actor, 2);
                skill = "skill02";
            }

            self.lastTime = now;

            GameEngine.GameApi.Send(new BattleActorAttackNotify() { uid = self.GetComponent<IdentityComponent>().objectId, skill = skill });

            for (int n = 0; n < targets.Count; ++n)
            {
                Actor actor = loadComponent.FindActorByUid(targets[n]);

                int damage = self.GetComponent<AttributeComponent>().attack - actor.GetComponent<AttributeComponent>().defense;
                actor.GetComponent<HardHurtComponent>().OnHurt(damage);
            }
        }

    }
}
