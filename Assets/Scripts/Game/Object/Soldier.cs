using GameEngine;

namespace Game
{
    public struct SoldierHurtDataInfo
    {
        public int damageType;
        public int damage;
        public string attackerName;
    }

    /// <summary>
    /// 士兵对象类
    /// </summary>
    [GameEngine.Inject]
    [GameEngine.PoolSupported]
    [EntityActivationComponent(typeof(AttributeComponent))]
    [EntityActivationComponent(typeof(AttackComponent))]
    public class Soldier : Actor, GameEngine.IUpdateActivation
    {
        private Buff buff;

        public Soldier()
        {
            Debugger.Warn("Soldier Construct Method Running ...");
        }

        ~Soldier()
        {
            Debugger.Warn("Soldier Destruct Method Running ...");
        }

        protected override void OnInitialize()
        {
            Debugger.Warn("soldier initialize !!!");
        }

        protected override void OnStartup()
        {
            Debugger.Warn("soldier startup !!!");
        }

        protected override void OnAwake()
        {
            Debugger.Warn("soldier awake !!!");
        }

        protected override void OnStart()
        {
            Debugger.Warn("soldier start !!!");

            //int zero = 0;
            //int f = 1 / zero;
            //Debugger.Warn($"zero = {zero}.");
        }

        protected override void OnUpdate()
        {
            // Debugger.Warn("soldier update !!!");
            //BodyComponent bc = GetComponent<BodyComponent>();
            //if (null != bc)
            //{
            //    RemoveComponent(bc);
            //}
        }

        protected override void OnDestroy()
        {
            Debugger.Warn("soldier destroy !!!");
        }

        protected override void OnShutdown()
        {
            Debugger.Warn("soldier shutdown !!!");
        }

        protected override void OnCleanup()
        {
            Debugger.Warn("soldier cleanup !!!");
        }

        public void OnBuffActivated()
        {
            if (null == buff)
            {
                Debugger.Info("soldier buff 为空,无法激活效果!");
            }
            else
            {
                buff.OnActivated();
            }
        }

        public void Hit(Actor actor)
        {
            AttributeComponent attackAttrComponent = GetComponent<AttributeComponent>();
            AttributeComponent targetAttrComponent = actor.GetComponent<AttributeComponent>();
            if (null == attackAttrComponent || null == targetAttrComponent)
            {
                return;
            }

            int damage = attackAttrComponent.attack - targetAttrComponent.defense;
            AttackComponent targetAttackComponent = actor.GetComponent<AttackComponent>();
            if (null == targetAttackComponent)
            { return; }

            targetAttackComponent.OnHurt(damage);
        }

        [EventSubscribeBindingOfTarget(1001)]
        public void OnHit(int eventID, params object[] args)
        {
            Debugger.Log("测试'Soldier'对象监听事件标识'{0}'的带参普通成员函数'OnHit'调用.", eventID);
        }

        [EventSubscribeBindingOfTarget(1001)]
        public void OnHitWithNullParameter()
        {
            Debugger.Log("测试'Soldier'对象监听事件标识'1001'的无参普通成员函数'OnHitWithNullParameter'调用.");
        }

        [EventSubscribeBindingOfTarget(typeof(SoldierHurtDataInfo))]
        public void OnHurt(SoldierHurtDataInfo eventData)
        {
            Debugger.Log("测试'Soldier'对象监听事件类型'{0}'的带参普通成员函数'OnHurt'调用, 参数值: damageType = {1}, damage = {2}, attackerName = {3}.",
                    eventData.GetType().FullName, eventData.damageType, eventData.damage, eventData.attackerName);
        }

        [EventSubscribeBindingOfTarget(typeof(SoldierHurtDataInfo))]
        public void OnHurtWithNullParameter()
        {
            Debugger.Log("测试'Soldier'对象监听事件类型'SoldierHurtDataInfo'的无参普通成员函数'OnHurtWithNullParameter'调用.");
        }

        [EventSubscribeBindingOfTarget(1202)]
        public void OnSoldierBiuBiuBiuWithNullParameter()
        {
            Debugger.Log("测试'Soldier'对象监听事件标识'1202'的无参普通成员函数'OnSoldierBiuBiuBiuWithNullParameter'调用.");
        }
    }
}
