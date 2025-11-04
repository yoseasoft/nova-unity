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
}
