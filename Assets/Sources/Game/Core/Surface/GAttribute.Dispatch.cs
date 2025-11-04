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
    /// 全局输入绑定函数的属性类型定义
    /// </summary>
    [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class OnGlobalInputAttribute : GameEngine.OnInputDispatchCallAttribute
    {
        public OnGlobalInputAttribute(int inputCode) : base(inputCode) { }

        public OnGlobalInputAttribute(SystemType inputDataType) : base(inputDataType) { }

        public OnGlobalInputAttribute(SystemType classType, int inputCode) : base(classType, inputCode) { }

        public OnGlobalInputAttribute(SystemType classType, SystemType inputDataType) : base(classType, inputDataType) { }
    }

    /// <summary>
    /// 对象输入绑定函数的属性类型定义
    /// </summary>
    [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class OnBeanInputAttribute : GameEngine.InputResponseBindingOfTargetAttribute
    {
        public OnBeanInputAttribute(int inputCode) : base(inputCode, GameEngine.AspectBehaviourType.Start) { }

        public OnBeanInputAttribute(SystemType inputDataType) : base(inputDataType, GameEngine.AspectBehaviourType.Start) { }
    }

    /// <summary>
    /// 全局事件绑定函数的属性类型定义
    /// </summary>
    [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class OnGlobalEventAttribute : GameEngine.OnEventDispatchCallAttribute
    {
        public OnGlobalEventAttribute(int eventID) : base(eventID) { }

        public OnGlobalEventAttribute(SystemType eventDataType) : base(eventDataType) { }

        public OnGlobalEventAttribute(SystemType classType, int eventID) : base(classType, eventID) { }

        public OnGlobalEventAttribute(SystemType classType, SystemType eventDataType) : base(classType, eventDataType) { }
    }

    /// <summary>
    /// 对象事件绑定函数的属性类型定义
    /// </summary>
    [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class OnBeanEventAttribute : GameEngine.EventSubscribeBindingOfTargetAttribute
    {
        public OnBeanEventAttribute(int eventID) : base(eventID, GameEngine.AspectBehaviourType.Start) { }

        public OnBeanEventAttribute(SystemType eventDataType) : base(eventDataType, GameEngine.AspectBehaviourType.Start) { }
    }

    /// <summary>
    /// 全局消息绑定函数的属性类型定义
    /// </summary>
    [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class OnGlobalMessageAttribute : GameEngine.OnMessageDispatchCallAttribute
    {
        public OnGlobalMessageAttribute(int opcode) : base(opcode) { }

        public OnGlobalMessageAttribute(SystemType messageType) : base(messageType) { }

        public OnGlobalMessageAttribute(SystemType classType, int opcode) : base(classType, opcode) { }

        public OnGlobalMessageAttribute(SystemType classType, SystemType messageType) : base(classType, messageType) { }
    }

    /// <summary>
    /// 对象消息绑定函数的属性类型定义
    /// </summary>
    [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class OnBeanMessageAttribute : GameEngine.MessageListenerBindingOfTargetAttribute
    {
        public OnBeanMessageAttribute(int opcode) : base(opcode, GameEngine.AspectBehaviourType.Start) { }

        public OnBeanMessageAttribute(SystemType messageType) : base(messageType, GameEngine.AspectBehaviourType.Start) { }
    }
}
