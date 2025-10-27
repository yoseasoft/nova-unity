using GameEngine;

namespace Game
{
    /// <summary>
    /// 怪物对象类
    /// </summary>
    [GameEngine.Inject]
    [CEntityAutomaticActivationComponent(typeof(AttributeComponent))]
    //[CEntityAutomaticActivationComponent(typeof(AttackComponent))]
    [CEntityAutomaticActivationComponent(typeof(HardHurtComponent))]
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
