/// <summary>
/// 2025-06-11 Game Framework Code By Hurley
/// </summary>

namespace Game
{
    /// <summary>
    /// 世界观察者自动绑定的特性标签<br/>
    /// 
    /// 定义该特性后，将通过函数名匹配的方式对该类的函数进行自动绑定
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class WorldObserverAutoBoundAttribute : System.Attribute
    {
    }

    /// <summary>
    /// 世界观察者相关函数引用的特性标签<br/>
    /// 
    /// 仅用于对调用处的函数与协议类型进行捆绑关联
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Field | System.AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class WorldObserverFunctionReferenceAttribute : System.Attribute
    {
        private readonly NovaEngine.Application.ProtocolType _protocolType;

        public NovaEngine.Application.ProtocolType ProtocolType => _protocolType;

        public WorldObserverFunctionReferenceAttribute(NovaEngine.Application.ProtocolType protocolType) : base()
        {
            _protocolType = protocolType;
        }
    }
}
