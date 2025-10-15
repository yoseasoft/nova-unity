using GameEngine;

namespace Game
{
    /// <summary>
    /// 属性组件对象类
    /// </summary>
    [GameEngine.Inject]
    public class AttributeComponent : CComponent
    {
        public int health;
        public int speed;
        public int attack;
        public int defense;

        public bool is_die;
    }
}
