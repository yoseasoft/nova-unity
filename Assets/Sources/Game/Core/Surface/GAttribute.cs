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
    /// 初始化阶段前置通知函数的属性类型定义
    /// </summary>
    [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class OnInitializeAttribute : GameEngine.OnAspectBeforeCallAttribute
    {
        public OnInitializeAttribute() : base(GameEngine.AspectBehaviourType.Initialize) { }
    }

    /// <summary>
    /// 开启阶段前置通知函数的属性类型定义
    /// </summary>
    [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class OnStartupAttribute : GameEngine.OnAspectBeforeCallAttribute
    {
        public OnStartupAttribute() : base(GameEngine.AspectBehaviourType.Startup) { }
    }

    /// <summary>
    /// 唤醒阶段前置通知函数的属性类型定义
    /// </summary>
    [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class OnAwakeAttribute : GameEngine.OnAspectBeforeCallAttribute
    {
        public OnAwakeAttribute() : base(GameEngine.AspectBehaviourType.Awake) { }
    }

    /// <summary>
    /// 开始阶段前置通知函数的属性类型定义
    /// </summary>
    [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class OnStartAttribute : GameEngine.OnAspectBeforeCallAttribute
    {
        public OnStartAttribute() : base(GameEngine.AspectBehaviourType.Start) { }
    }

    /// <summary>
    /// 执行阶段前置通知函数的属性类型定义
    /// </summary>
    [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class OnExecuteAttribute : GameEngine.OnAspectBeforeCallAttribute
    {
        public OnExecuteAttribute() : base(GameEngine.AspectBehaviourType.Execute) { }
    }

    /// <summary>
    /// 延迟执行阶段前置通知函数的属性类型定义
    /// </summary>
    [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class OnLateExecuteAttribute : GameEngine.OnAspectBeforeCallAttribute
    {
        public OnLateExecuteAttribute() : base(GameEngine.AspectBehaviourType.LateExecute) { }
    }

    /// <summary>
    /// 刷新阶段前置通知函数的属性类型定义
    /// </summary>
    [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class OnUpdateAttribute : GameEngine.OnAspectBeforeCallAttribute
    {
        public OnUpdateAttribute() : base(GameEngine.AspectBehaviourType.Update) { }
    }

    /// <summary>
    /// 延迟刷新阶段前置通知函数的属性类型定义
    /// </summary>
    [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class OnLateUpdateAttribute : GameEngine.OnAspectBeforeCallAttribute
    {
        public OnLateUpdateAttribute() : base(GameEngine.AspectBehaviourType.LateUpdate) { }
    }

    /// <summary>
    /// 销毁阶段后置通知函数的属性类型定义
    /// </summary>
    [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class OnDestroyAttribute : GameEngine.OnAspectAfterCallAttribute
    {
        public OnDestroyAttribute() : base(GameEngine.AspectBehaviourType.Destroy) { }
    }

    /// <summary>
    /// 关闭阶段后置通知函数的属性类型定义
    /// </summary>
    [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class OnShutdownAttribute : GameEngine.OnAspectAfterCallAttribute
    {
        public OnShutdownAttribute() : base(GameEngine.AspectBehaviourType.Shutdown) { }
    }

    /// <summary>
    /// 清理阶段后置通知函数的属性类型定义
    /// </summary>
    [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class OnCleanupAttribute : GameEngine.OnAspectAfterCallAttribute
    {
        public OnCleanupAttribute() : base(GameEngine.AspectBehaviourType.Cleanup) { }
    }

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
    public class OnObjectInputAttribute : GameEngine.InputResponseBindingOfTargetAttribute
    {
        public OnObjectInputAttribute(int inputCode) : base(inputCode, GameEngine.AspectBehaviourType.Start) { }

        public OnObjectInputAttribute(SystemType inputDataType) : base(inputDataType, GameEngine.AspectBehaviourType.Start) { }
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
    public class OnObjectEventAttribute : GameEngine.EventSubscribeBindingOfTargetAttribute
    {
        public OnObjectEventAttribute(int eventID) : base(eventID, GameEngine.AspectBehaviourType.Start) { }

        public OnObjectEventAttribute(SystemType eventDataType) : base(eventDataType, GameEngine.AspectBehaviourType.Start) { }
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
    public class OnObjectMessageAttribute : GameEngine.MessageListenerBindingOfTargetAttribute
    {
        public OnObjectMessageAttribute(int opcode) : base(opcode, GameEngine.AspectBehaviourType.Start) { }

        public OnObjectMessageAttribute(SystemType messageType) : base(messageType, GameEngine.AspectBehaviourType.Start) { }
    }

    /// <summary>
    /// 编程接口分发类型注册函数的属性类型定义
    /// </summary>
    [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class OnApiFunctionAttribute : GameEngine.OnApiDispatchCallAttribute
    {
        public OnApiFunctionAttribute(string functionName) : base(functionName) { }

        public OnApiFunctionAttribute(SystemType classType, string functionName) : base(classType, functionName) { }
    }
}
