using GameEngine;

namespace Game
{
    /// <summary>
    /// 角色对象基类
    /// </summary>
    [GameEngine.Inject]
    [EntityActivationComponent(typeof(TransformComponent))]
    [EntityActivationComponent(typeof(MoveComponent))]
    public class Actor : CObject
    {
        public Actor()
        {
            Debugger.Warn("Actor ({0}) Construct Method Running ...", GetType().FullName);
        }

        ~Actor()
        {
            Debugger.Warn("Actor ({0}) Destruct Method Running ...", GetType().FullName);
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
