using GameEngine;

namespace Game
{
    /// <summary>
    /// 角色对象基类
    /// </summary>
    [GameEngine.Inject]
    [EntityActivationComponent(typeof(IdentityComponent))]
    [EntityActivationComponent(typeof(TransformComponent))]
    [EntityActivationComponent(typeof(MoveComponent))]
    public class Actor : CActor
    {
        public Actor()
        {
            Debugger.Warn("当前正在运行 Actor ({%t}) 对象的构造函数……", this);
        }

        ~Actor()
        {
            Debugger.Warn("当前正在运行 Actor ({%t}) 对象的析构函数……", this);
        }

        public void MoveTo(UnityEngine.Vector3 position)
        {
            MoveComponent moveComponent = GetComponent<MoveComponent>();
            if (null != moveComponent)
            {
                moveComponent.MoveTo(position);
            }
        }
    }
}
