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
    /// 编程接口分发类型注册函数的属性类型定义
    /// </summary>
    [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class OnApiFunctionAttribute : GameEngine.OnApiDispatchCallAttribute
    {
        public OnApiFunctionAttribute(string functionName) : base(functionName) { }

        public OnApiFunctionAttribute(SystemType classType, string functionName) : base(classType, functionName) { }
    }
}
