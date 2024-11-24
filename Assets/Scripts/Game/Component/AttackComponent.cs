using GameEngine;

namespace Game
{
    /// <summary>
    /// 攻击组件对象类
    /// </summary>
    [GameEngine.Inject]
    public class AttackComponent : CComponent
    {
        public Bullet bullet;
        public void OnAttack()
        {
            if (null == bullet)
            {
                Debugger.Log("普通攻击打向目标对象!");
            }
            else
            {
                bullet.Fire();
            }
        }
        public void OnHurt(int damage)
        {
            AttributeComponent attr = GetComponent<AttributeComponent>();
            if (null != attr)
            {
                attr.health = System.Math.Max(0, attr.health - damage);
            }
        }
    }
}
