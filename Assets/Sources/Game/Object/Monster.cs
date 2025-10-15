using GameEngine;

namespace Game
{
    /// <summary>
    /// 怪物对象类
    /// </summary>
    [GameEngine.Inject]
    [EntityActivationComponent(typeof(AttributeComponent))]
    //[EntityActivationComponent(typeof(AttackComponent))]
    [EntityActivationComponent(typeof(HardHurtComponent))]
    public class Monster : Actor
    {
        protected override void OnAwake()
        {
            Debugger.Warn("monster awake !!!");
        }

        protected override void OnStart()
        {
            Debugger.Warn("monster start !!!");
        }

        protected override void OnUpdate()
        {
            // Debugger.Warn("monster update !!!");
        }
    }
}
