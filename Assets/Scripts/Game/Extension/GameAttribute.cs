/// <summary>
/// 2024-05-10 Game Framework Code By Hurley
/// </summary>

using SystemType = System.Type;
using SystemAttribute = System.Attribute;
using SystemAttributeUsageAttribute = System.AttributeUsageAttribute;
using SystemAttributeTargets = System.AttributeTargets;

namespace Game
{
    /// <summary>
    /// 唤醒阶段后置通知函数的属性类型定义
    /// </summary>
    [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class OnAwakeAttribute : GameEngine.OnAspectAfterCallAttribute
    {
        public OnAwakeAttribute() : base(GameEngine.AspectBehaviourType.Awake) { }
    }

    /// <summary>
    /// 开始阶段后置通知函数的属性类型定义
    /// </summary>
    [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class OnStartAttribute : GameEngine.OnAspectAfterCallAttribute
    {
        public OnStartAttribute() : base(GameEngine.AspectBehaviourType.Start) { }
    }

    /// <summary>
    /// 刷新阶段后置通知函数的属性类型定义
    /// </summary>
    [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class OnUpdateAttribute : GameEngine.OnAspectAfterCallAttribute
    {
        public OnUpdateAttribute() : base(GameEngine.AspectBehaviourType.Update) { }
    }

    /// <summary>
    /// 延迟刷新阶段后置通知函数的属性类型定义
    /// </summary>
    [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class OnLateUpdateAttribute : GameEngine.OnAspectAfterCallAttribute
    {
        public OnLateUpdateAttribute() : base(GameEngine.AspectBehaviourType.LateUpdate) { }
    }

    /// <summary>
    /// 销毁阶段后置通知函数的属性类型定义
    /// </summary>
    [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class OnDestroyAttribute : GameEngine.OnAspectAfterCallAttribute
    {
        public OnDestroyAttribute() : base(GameEngine.AspectBehaviourType.Destroy) { }
    }

    /// <summary>
    /// 事件订阅绑定函数的属性类型定义
    /// </summary>
    [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class OnEventSubscribingAttribute : GameEngine.EventSubscribeBindingOfTargetAttribute
    {
        public OnEventSubscribingAttribute(int eventID) : base(eventID, GameEngine.AspectBehaviourType.Start) { }

        public OnEventSubscribingAttribute(SystemType eventDataType) : base(eventDataType, GameEngine.AspectBehaviourType.Start) { }
    }

    /// <summary>
    /// 消息监听绑定函数的属性类型定义
    /// </summary>
    [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class OnMessageListeningAttribute : GameEngine.MessageListenerBindingOfTargetAttribute
    {
        public OnMessageListeningAttribute(int opcode) : base(opcode, GameEngine.AspectBehaviourType.Start) { }

        public OnMessageListeningAttribute(SystemType messageType) : base(messageType, GameEngine.AspectBehaviourType.Start) { }
    }
}
